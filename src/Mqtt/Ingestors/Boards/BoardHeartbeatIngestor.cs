using HomeAutomation.Database;
using HomeAutomation.Models.Database;
using HomeAutomation.Mqtt.Services;
using LinqToDB;
using MQTTnet;
using System.Text.Json;

namespace HomeAutomation.Mqtt.Ingestors.Boards;

internal sealed class BoardHeartbeatIngestor : IngestBase
{
    private readonly IDatabaseCache _databaseCache;

    public BoardHeartbeatIngestor(
        MqttClientOptions options,
        MqttClientFactory clientFactory,
        IServiceScopeFactory serviceScopeFactory,
        IDatabaseCache databaseCache,
        ILogger<BoardHeartbeatIngestor> logger)
        : base("board/+/heartbeat", "Ha-BoardHeartbeatIngest", options, clientFactory, serviceScopeFactory, logger)
    {
        _databaseCache = databaseCache;
    }

    protected override async Task OnMessageReceived(string payload, MqttApplicationMessageReceivedEventArgs e)
    {
        if (string.IsNullOrEmpty(payload))
        {
            _logger.LogWarning("Received empty payload for board heartbeat.");
            return;
        }

        Models.Mqtt.Heartbeat? heartbeat;

        try
        {
            heartbeat = JsonSerializer.Deserialize<Models.Mqtt.Heartbeat>(payload, _serializationOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize heartbeat payload: {Payload}", payload);
            return;
        }

        if (heartbeat is null)
        {
            _logger.LogWarning("Failed to deserialize heartbeat from payload: {Payload}", payload);
            return;
        }

        Board? board = await _databaseCache.GetBoardAsync(heartbeat.BoardId.ToString());
        if (board is null)
        {
            _logger.LogWarning("Heartbeat received for unknown board: {BoardId}", heartbeat.BoardId);
            return;
        }

        try
            {
                _logger.LogInformation("Inserting heartbeat into database: {Heartbeat}", heartbeat);
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                using HomeAutomationDb dbContext = scope.ServiceProvider.GetRequiredService<HomeAutomationDb>();
                await dbContext.InsertAsync(new Models.Database.Heartbeat
                {
                    BoardSerialNumber = heartbeat.BoardId.ToString(),
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