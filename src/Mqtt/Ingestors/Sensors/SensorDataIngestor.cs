using HomeAutomation.Database;
using LinqToDB;
using MQTTnet;
using System.Text.Json;

namespace HomeAutomation.Mqtt.Ingestors.Sensors;

internal sealed class SensorDataIngestor : IngestBase
{
    public SensorDataIngestor(
        MqttClientOptions options,
        MqttClientFactory clientFactory,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<SensorDataIngestor> logger)
        : base("board/+/sensor/#", options, clientFactory, serviceScopeFactory, logger)
    {
    }

    protected override async Task OnMessageReceived(string payload, MqttApplicationMessageReceivedEventArgs e)
    {
        if (string.IsNullOrEmpty(payload))
        {
            _logger.LogWarning("Received empty payload for sensor value.");
            return;
        }

        Models.Mqtt.DataPayload? sensorData = JsonSerializer.Deserialize<Models.Mqtt.DataPayload>(payload, _serializationOptions);
        if (sensorData is null)
        {
            _logger.LogWarning("Failed to deserialize sensor data from payload: {Payload}", payload);
            return;
        }

        int sensorId = 0;
        if (int.TryParse(sensorData.SensorId, out sensorId) == false)
        {
            _logger.LogWarning("Invalid sensor ID format: '{SensorId}' for payload: {Payload}", sensorData.SensorId, payload);
            return;
        }

        try
        {
            _logger.LogInformation("Inserting sensor data into database: {SensorData}", sensorData);
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            using HomeAutomationDb dbContext = scope.ServiceProvider.GetRequiredService<HomeAutomationDb>();
            await dbContext.InsertAsync(new Models.Database.SensorValue
            {
                BoardId = sensorData.BoardId,
                Timestamp = sensorData.Timestamp,
                Value = sensorData.SensorValue,
                SensorId = sensorId,
            });
            _logger.LogInformation("Successfully inserted sensor data for board {BoardId} at {Timestamp}.", sensorData.BoardId, sensorData.Timestamp);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to insert sensor data into database.");
        }
    }
}