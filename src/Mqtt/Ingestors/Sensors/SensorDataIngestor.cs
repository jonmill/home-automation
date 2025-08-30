using HomeAutomation.Database;
using HomeAutomation.Models.Database;
using HomeAutomation.MqttExtensions;
using HomeAutomation.PushExtensions;
using LinqToDB;
using MQTTnet;
using System.Text.Json;

namespace HomeAutomation.Mqtt.Ingestors.Sensors;

internal sealed class SensorDataIngestor : IngestBase
{
    private readonly IDatabaseCache _databaseCache;
    private readonly IPushNotifier _pushClient;

    public SensorDataIngestor(
        MqttClientOptions options,
        MqttClientFactory clientFactory,
        IServiceScopeFactory serviceScopeFactory,
        IDatabaseCache databaseCache,
        IPushNotifier pushClient,
        ILogger<SensorDataIngestor> logger)
        : base("board/+/sensor/#", "Ha-SensorsIngest", options, clientFactory, serviceScopeFactory, logger)
    {
        _databaseCache = databaseCache;
        _pushClient = pushClient;
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

            await SendNewDataMessageAsync(Models.Mqtt.NewDataEventTypes.SensorData);
            _logger.LogInformation("Sent new data MQTT message for sensor data.");

            string title = string.Empty;
            string message = string.Empty;
            if (sensor.Type == Models.Enums.SensorTypes.Contact)
            {
                title = $"Door {(sensorData.SensorValue == "1" ? "Opened" : "Closed")}";
                message = $"Door {sensor.Name} is now {(sensorData.SensorValue == "1" ? "open" : "closed")}.";
            }
            else if ((sensor.Type == Models.Enums.SensorTypes.AirQuality) && (double.TryParse(sensorData.SensorValue, out double airQualityValue)) && (airQualityValue > 50))
            {
                title = "Air Quality Alert";

                if (sensor.Id == 13)
                {
                    message = $"Nora's Room Air Quality now at {sensorData.SensorValue}.";
                }
                else
                {
                    message = $"{sensor.Name} Air Quality now at {sensorData.SensorValue}";
                }
            }
            else if ((sensor.Type == Models.Enums.SensorTypes.Temperature) && (double.TryParse(sensorData.SensorValue, out double temperatureValue)) && (temperatureValue >= 23.88))
            {
                title = "High Temperature Alert";
                message = $"{sensor.Name} Temperature now at {sensorData.SensorValue}.";
            }

            if ((string.IsNullOrEmpty(title) == false) && (string.IsNullOrEmpty(message) == false))
            {
                await _pushClient.NotifyAsync(title, message);
                _logger.LogInformation("Sent push notification for sensor data: {Title}", title);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to insert sensor data into database.");
        }
    }
}