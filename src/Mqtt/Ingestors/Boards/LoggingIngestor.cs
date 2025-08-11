using HomeAutomation.Database;
using LinqToDB;
using MQTTnet;
using System.Text.Json;

namespace HomeAutomation.Mqtt.Ingestors.Boards;

internal sealed class LoggingIngestor : IngestBase
{
    public LoggingIngestor(
        MqttClientOptions options,
        MqttClientFactory clientFactory,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<LoggingIngestor> logger)
        : base("board/+/logging", options, clientFactory, serviceScopeFactory, logger)
    {
    }

    protected override async Task OnMessageReceived(string payload, MqttApplicationMessageReceivedEventArgs e)
    {
        if (string.IsNullOrEmpty(payload))
        {
            _logger.LogWarning("Received empty payload for logging.");
            return;
        }

        Models.Mqtt.LogEvent? logEntry = JsonSerializer.Deserialize<Models.Mqtt.LogEvent>(payload, _serializationOptions);
        if (logEntry is null)
        {
            _logger.LogWarning("Failed to deserialize log entry from payload: {Payload}", payload);
            return;
        }

        try
        {
            _logger.LogInformation("Inserting log entry into database: {LogEntry}", logEntry);
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            using HomeAutomationDb dbContext = scope.ServiceProvider.GetRequiredService<HomeAutomationDb>();
            await dbContext.InsertAsync(new Models.Database.LogEntry
            {
                BoardId = logEntry.BoardId,
                Timestamp = logEntry.Timestamp,
                Level = logEntry.Level,
                Message = logEntry.Message ?? "[Null]",
                Tag = logEntry.Tag ?? "[Null]",
            });
            _logger.LogInformation("Successfully inserted log entry for board {BoardId} at {Timestamp}.", logEntry.BoardId, logEntry.Timestamp);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to insert log entry into database.");
        }
    }
}