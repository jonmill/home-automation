using HomeAutomation.MqttExtensions;
using MQTTnet;

namespace HomeAutomation.Mqtt.Ingestors.Boards;

internal sealed class PowerStateIngestor : IngestBase
{
    public PowerStateIngestor(
        MqttClientOptions options,
        MqttClientFactory clientFactory,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<PowerStateIngestor> logger)
        : base("board/+/power", "Ha-BoardPowerIngest", options, clientFactory, serviceScopeFactory, logger)
    {
    }

    protected override Task OnMessageReceived(string payload, MqttApplicationMessageReceivedEventArgs e)
    {
        if (string.IsNullOrEmpty(payload))
        {
            _logger.LogWarning("Received empty payload for Power state update.");
            return Task.CompletedTask;
        }

        _logger.LogInformation("Received Power state update: {Payload}", payload);
        return Task.CompletedTask;
    }
}