using HomeAutomation.Database;
using HomeAutomation.Models.Database;
using HomeAutomation.Models.Mqtt;
using HomeAutomation.MqttExtensions;
using LinqToDB;
using MQTTnet;
using System.Text.Json;

namespace HomeAutomation.Mqtt.Ingestors.Boards;

internal sealed class LoggingIngestor : IngestBase
{
    private readonly IDatabaseCache _databaseCache;

    public LoggingIngestor(
        MqttClientOptions options,
        MqttClientFactory clientFactory,
        IServiceScopeFactory serviceScopeFactory,
        IDatabaseCache databaseCache,
        ILogger<LoggingIngestor> logger)
        : base("board/+/logging", "Ha-BoardLoggerIngest", options, clientFactory, serviceScopeFactory, logger)
    {
        _databaseCache = databaseCache;
    }

    protected override async Task OnMessageReceived(string payload, MqttApplicationMessageReceivedEventArgs e)
    {
        if (string.IsNullOrEmpty(payload))
        {
            _logger.LogWarning("Received empty payload for logging.");
            return;
        }

        Models.Mqtt.LogEvent? logEntry;

        try
        {
            logEntry = JsonSerializer.Deserialize<Models.Mqtt.LogEvent>(payload, _serializationOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize log entry payload: {Payload}", payload);
            return;
        }

        if (logEntry is null)
        {
            _logger.LogWarning("Failed to deserialize log entry from payload: {Payload}", payload);
            return;
        }

        Board? board = await _databaseCache.GetBoardAsync(logEntry.BoardId.ToString());
        if (board is null)
        {
            _logger.LogWarning("Log entry received for unknown board: {BoardId}", logEntry.BoardId);
            return;
        }

        try
        {
            _logger.LogInformation("Inserting log entry into database: {LogEntry}", logEntry);
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            using HomeAutomationDb dbContext = scope.ServiceProvider.GetRequiredService<HomeAutomationDb>();
            await dbContext.InsertAsync(new Models.Database.LogEntry
            {
                BoardSerialNumber = logEntry.BoardId.ToString(),
                Timestamp = logEntry.Timestamp,
                Level = logEntry.Level,
                Message = logEntry.Message ?? "[Null]",
                Tag = logEntry.Tag ?? "[Null]",
            });
            await SendNewDataMessageAsync(NewDataEventTypes.LogEvent);
            _logger.LogInformation("Successfully inserted log entry for board {BoardId} at {Timestamp}.", logEntry.BoardId, logEntry.Timestamp);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to insert log entry into database.");
        }
    }
}