using HomeAutomation.Database;
using LinqToDB;
using MQTTnet;
using System.Text.Json;

namespace HomeAutomation.Mqtt.Ingestors.Boards;

internal sealed class BoardHeartbeatIngestor : IngestBase
{
    public BoardHeartbeatIngestor(
        MqttClientOptions options,
        MqttClientFactory clientFactory,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<BoardHeartbeatIngestor> logger)
        : base("board/+/heartbeat", options, clientFactory, serviceScopeFactory, logger)
    {
    }

    protected override async Task OnMessageReceived(string payload, MqttApplicationMessageReceivedEventArgs e)
    {
        if (string.IsNullOrEmpty(payload))
        {
            _logger.LogWarning("Received empty payload for board heartbeat.");
            return;
        }

        Models.Mqtt.Heartbeat? heartbeat = JsonSerializer.Deserialize<Models.Mqtt.Heartbeat>(payload, _serializationOptions);
        if (heartbeat is null)
        {
            _logger.LogWarning("Failed to deserialize heartbeat from payload: {Payload}", payload);
            return;
        }

        try
        {
            _logger.LogInformation("Inserting heartbeat into database: {Heartbeat}", heartbeat);
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            using HomeAutomationDb dbContext = scope.ServiceProvider.GetRequiredService<HomeAutomationDb>();
            await dbContext.InsertAsync(new Models.Database.Heartbeat
            {
                BoardId = heartbeat.BoardId,
                Timestamp = heartbeat.Timestamp,
                NextExpectedHeartbeat = heartbeat.Timestamp.AddSeconds(heartbeat.NextExpectedHeartbeatInSeconds),
            });
            _logger.LogInformation("Successfully inserted heartbeat for board {BoardId} at {Timestamp}.", heartbeat.BoardId, heartbeat.Timestamp);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to insert heartbeat into database.");
        }
    }
}