using HomeAutomation.Models.Mqtt.Ingest;
using MQTTnet;

namespace HomeAutomation.Mqtt.Ingestors.Boards;

internal sealed class InternalTemperatureIngestor : IngestBase
{
    public InternalTemperatureIngestor(
        MqttClientOptions options,
        MqttClientFactory clientFactory,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<InternalTemperatureIngestor> logger)
        : base("board/+/internal_temperature", "Ha-BoardTempIngest", options, clientFactory, serviceScopeFactory, logger)
    {
    }

    protected override Task OnMessageReceived(string payload, MqttApplicationMessageReceivedEventArgs e)
    {
        if (string.IsNullOrEmpty(payload))
        {
            _logger.LogWarning("Received empty payload for board temperature.");
            return Task.CompletedTask;
        }

        _logger.LogInformation("Received board internal temperature update: {Payload}", payload);
        return Task.CompletedTask;
    }
}