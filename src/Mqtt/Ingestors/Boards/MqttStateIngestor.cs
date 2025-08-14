using MQTTnet;

namespace HomeAutomation.Mqtt.Ingestors.Boards;

internal sealed class MqttStateIngestor : IngestBase
{
    public MqttStateIngestor(
        MqttClientOptions options,
        MqttClientFactory clientFactory,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<MqttStateIngestor> logger)
        : base("board/+/mqtt", "Ha-MqttStateIngesr", options, clientFactory, serviceScopeFactory, logger)
    {
    }

    protected override Task OnMessageReceived(string payload, MqttApplicationMessageReceivedEventArgs e)
    {
        if (string.IsNullOrEmpty(payload))
        {
            _logger.LogWarning("Received empty payload for MQTT state update.");
            return Task.CompletedTask;
        }

        _logger.LogInformation("Received MQTT state update: {Payload}", payload);
        return Task.CompletedTask;
    }
}