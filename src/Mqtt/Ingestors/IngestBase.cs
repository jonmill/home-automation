using System.Text.Json;
using MQTTnet;

namespace HomeAutomation.Mqtt.Ingestors;

internal abstract class IngestBase : IHostedService, IAsyncDisposable
{
    private readonly MqttClientOptions _options;
    private readonly MqttClientFactory _clientFactory;
    private readonly MqttClientSubscribeOptions _subscriptionOptions;
    private readonly MqttClientUnsubscribeOptions _unsubscribeOptions;
    protected readonly JsonSerializerOptions _serializationOptions;
    protected readonly IServiceScopeFactory _serviceScopeFactory;
    protected readonly ILogger<IngestBase> _logger;
    private IMqttClient? _client;

    public IngestBase(
        string topic,
        MqttClientOptions options,
        MqttClientFactory clientFactory,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<IngestBase> logger)
    {
        _client = clientFactory.CreateMqttClient();
        _options = options;
        _clientFactory = clientFactory;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _subscriptionOptions = _clientFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(_clientFactory.CreateTopicFilterBuilder().WithTopic(topic).Build())
            .Build();
        _unsubscribeOptions = _clientFactory.CreateUnsubscribeOptionsBuilder()
            .WithTopicFilter(_clientFactory.CreateTopicFilterBuilder().WithTopic(topic).Build())
            .Build();
        _serializationOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowOutOfOrderMetadataProperties = true,
        };
    }

    public async ValueTask DisposeAsync()
    {
        if (_client is not null)
        {
            _logger.LogDebug("Disposing of MQTT client.");
            await _client.TryDisconnectAsync(MqttClientDisconnectOptionsReason.NormalDisconnection);
            _client.Dispose();
            _client = null;
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_client is null)
        {
            _logger.LogError("MqttClient is not initialized in StartAsync");
            return;
        }

        // Figure out if we can do this with one client or do we need many?
        _logger.LogInformation("Starting MQTT client for {Topic}.", _subscriptionOptions.TopicFilters.First().Topic);
        _client.ApplicationMessageReceivedAsync += InternalOnMessageReceived;
        await _client.ConnectAsync(_options, cancellationToken);
        await _client.SubscribeAsync(_subscriptionOptions, cancellationToken);
        _logger.LogInformation("Subscribed to {Topic}.", _subscriptionOptions.TopicFilters.First().Topic);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_client is null)
        {
            _logger.LogWarning("MqttClient is not initialized in StopAsync");
            return;
        }

        _logger.LogInformation("Stopping MQTT client for {Topic}.", _subscriptionOptions.TopicFilters.First().Topic);
        _client.ApplicationMessageReceivedAsync -= InternalOnMessageReceived;
        await _client.UnsubscribeAsync(_unsubscribeOptions, cancellationToken);
        await _client.DisconnectAsync(MqttClientDisconnectOptionsReason.NormalDisconnection, cancellationToken: cancellationToken);
        _logger.LogInformation("Unsubscribed from {Topic} and disconnected.", _subscriptionOptions.TopicFilters.First().Topic);
    }

    private Task InternalOnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
    {
        if (e.ApplicationMessage.Payload.IsEmpty || (e.ApplicationMessage.Payload.Length == 0))
        {
            _logger.LogWarning("Received empty MQTT message from {Topic}", e.ApplicationMessage.Topic);
            return Task.CompletedTask;
        }
        else
        {
            _logger.LogInformation("Received MQTT message from {Topic}", e.ApplicationMessage.Topic);
            string payload = e.ApplicationMessage.ConvertPayloadToString();
            payload = payload.Replace("\x1b", string.Empty);
            return OnMessageReceived(payload, e);
        }
    }

    protected abstract Task OnMessageReceived(string payload, MqttApplicationMessageReceivedEventArgs e);
}