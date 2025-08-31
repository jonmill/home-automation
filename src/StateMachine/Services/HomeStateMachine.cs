using HomeAutomation.Database;
using HomeAutomation.Models.Database;
using HomeAutomation.Models.Mqtt;
using HomeAutomation.MqttExtensions;
using HomeAutomation.PushExtensions;
using HomeAutomation.StateMachine.Models;
using MQTTnet;
using System.Text.Json;

namespace HomeAutomation.StateMachine.Services;

public sealed class HomeStateMachine : IngestBase
{
    private readonly IDatabaseCache _databaseCache;
    private readonly IPushNotifier _pushNotifier;
    private readonly HomeModel _model;

    public HomeStateMachine(
        IDatabaseCache dbCache,
        IPushNotifier pushNotifier,
        HomeModel model,
        MqttClientOptions options,
        MqttClientFactory clientFactory,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<IngestBase> logger)
        : base(
            ["ring/+/alarm/+/contact/state", "board/+/sensor/+/contact_state"],
            "Ha-HomeStateMachine",
            options,
            clientFactory,
            serviceScopeFactory,
            logger)
    {
        _databaseCache = dbCache;
        _pushNotifier = pushNotifier;
        _model = model;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        // Initialize our home state machine with the current state of the sensors
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        using HomeAutomationDb database = scope.ServiceProvider.GetRequiredService<HomeAutomationDb>();
        await _model.InitializeAsync(database);

        // Call into the base Ingest to begin MQTT ingest
        await base.StartAsync(cancellationToken);
    }

    protected override Task OnMessageReceived(string payload, MqttApplicationMessageReceivedEventArgs e)
    {
        if (string.IsNullOrEmpty(payload))
        {
            _logger.LogWarning("Received empty payload on topic: {Topic}", e.ApplicationMessage.Topic);
            return Task.CompletedTask;
        }
        else if (e.ApplicationMessage.Topic.StartsWith("ring/", StringComparison.OrdinalIgnoreCase))
        {
            return HandleRingContactStateChangeAsync(payload, e.ApplicationMessage.Topic);
        }
        else if (e.ApplicationMessage.Topic.StartsWith("board/", StringComparison.OrdinalIgnoreCase))
        {
            return HandleEspHomeContactStateChangeAsync(payload);
        }
        else
        {
            _logger.LogWarning("Received message on unknown topic: {Topic}", e.ApplicationMessage.Topic);
            return Task.CompletedTask;
        }
    }

    private async Task HandleRingContactStateChangeAsync(string payload, string topic)
    {
        string[] parts = topic.Split('/');
        if (parts.Length < 6)
        {
            _logger.LogWarning("Invalid topic format for Ring Contact State: {Topic}", topic);
            return;
        }

        string serialNumber = parts[3];
        Sensor? sensor = await _databaseCache.GetSensorAsync(serialNumber);
        if (sensor is null)
        {
            _logger.LogWarning("Sensor with serial number {SerialNumber} not found in database.", serialNumber);
            return;
        }

        bool isClosed = string.Equals(payload, "off", StringComparison.OrdinalIgnoreCase);
        _logger.LogInformation("Received Ring Contact State for {SerialNumber}: Is Closed = {State}", serialNumber, isClosed);

        try
        {
            await _model.UpdateSensorStateAsync(sensor, !isClosed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating sensor state for {SerialNumber}", serialNumber);
            return;
        }
    }

    private async Task HandleEspHomeContactStateChangeAsync(string payload)
    {
        DataPayload? sensorData;

        try
        {
            sensorData = JsonSerializer.Deserialize<DataPayload>(payload, _serializationOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize sensor data payload: {Payload}", payload);
            return;
        }

        if (sensorData is null)
        {
            _logger.LogWarning("Failed to deserialize sensor data from payload: {Payload}", payload);
            return;
        }

        Sensor? sensor = await _databaseCache.GetSensorAsync(sensorData.SensorId);
        if (sensor is null)
        {
            _logger.LogWarning("Sensor data received for unknown sensor: {SensorId}", sensorData.SensorId);
            return;
        }

        try
        {
            await _model.UpdateSensorStateAsync(sensor, sensorData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating sensor state for {SensorId}", sensorData.SensorId);
            return;
        }
    }
}