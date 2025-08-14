using HomeAutomation.Database;
using HomeAutomation.Models.Database;
using HomeAutomation.Mqtt.Services;
using LinqToDB;
using MQTTnet;
using System.Text.Json;

namespace HomeAutomation.Mqtt.Ingestors.Sensors;

internal sealed class SensorDataIngestor : IngestBase
{
    private readonly IDatabaseCache _databaseCache;

    public SensorDataIngestor(
        MqttClientOptions options,
        MqttClientFactory clientFactory,
        IServiceScopeFactory serviceScopeFactory,
        IDatabaseCache databaseCache,
        ILogger<SensorDataIngestor> logger)
        : base("board/+/sensor/#", "Ha-SensorsIngest", options, clientFactory, serviceScopeFactory, logger)
    {
        _databaseCache = databaseCache;
    }

    protected override async Task OnMessageReceived(string payload, MqttApplicationMessageReceivedEventArgs e)
    {
        if (string.IsNullOrEmpty(payload))
        {
            _logger.LogWarning("Received empty payload for sensor value.");
            return;
        }

        Models.Mqtt.DataPayload? sensorData;

        try
        {
            sensorData = JsonSerializer.Deserialize<Models.Mqtt.DataPayload>(payload, _serializationOptions);
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
            _logger.LogInformation("Inserting sensor data into database: {SensorData}", sensorData);
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            using HomeAutomationDb dbContext = scope.ServiceProvider.GetRequiredService<HomeAutomationDb>();
            await dbContext.InsertAsync(new Models.Database.SensorValue
            {
                BoardSerialNumber = sensorData.BoardId.ToString(),
                Timestamp = sensorData.Timestamp,
                Value = sensorData.SensorValue,
                SensorSerialNumber = sensorData.SensorId,
            });
            _logger.LogInformation("Successfully inserted sensor data for board {BoardId} at {Timestamp}.", sensorData.BoardId, sensorData.Timestamp);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to insert sensor data into database.");
        }
    }
}