using HomeAutomation.Models.Mqtt.Ingest;
using MQTTnet;

namespace HomeAutomation.Mqtt.Ingestors.Boards;

internal sealed class OtaStateIngestor : IngestBase
{
    public OtaStateIngestor(
        MqttClientOptions options,
        MqttClientFactory clientFactory,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<OtaStateIngestor> logger)
        : base("board/+/ota", "Ha-BoardOtaIngest", options, clientFactory, serviceScopeFactory, logger)
    {
    }

    protected override Task OnMessageReceived(string payload, MqttApplicationMessageReceivedEventArgs e)
    {
        if (string.IsNullOrEmpty(payload))
        {
            _logger.LogWarning("Received empty payload for OTA state update.");
            return Task.CompletedTask;
        }

        _logger.LogInformation("Received OTA state update: {Payload}", payload);
        return Task.CompletedTask;
    }
}