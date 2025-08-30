using HomeAutomation.Database;
using HomeAutomation.Models.Mqtt;
using HomeAutomation.MqttExtensions;
using MQTTnet;
using System.Text.Json;

namespace HomeAutomation.Web.Services;

internal sealed class DataUpdateService : IngestBase
{
    private readonly UserSessionsRepository _userSessionsRepository;

    public DataUpdateService(
        MqttClientOptions options,
        MqttClientFactory clientFactory,
        IServiceScopeFactory serviceScopeFactory,
        IDatabaseCache databaseCache,
        UserSessionsRepository userSessionsRepository,
        ILogger<DataUpdateService> logger) : base("subscriptions/new-data", "Ha-DataUpdateService", options, clientFactory, serviceScopeFactory, logger)
    {
        _userSessionsRepository = userSessionsRepository;
    }

    protected override async Task OnMessageReceived(string payload, MqttApplicationMessageReceivedEventArgs e)
    {
        if (string.IsNullOrEmpty(payload))
        {
            _logger.LogWarning("Received empty payload for new data notification.");
            return;
        }

        NewData? notification;

        try
        {
            notification = JsonSerializer.Deserialize<NewData>(payload, _serializationOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize new data payload: {Payload}", payload);
            return;
        }

        if (notification is null)
        {
            _logger.LogWarning("Failed to deserialize new data from payload: {Payload}", payload);
            return;
        }

        await UpdateDataAsync();
    }

    private async Task UpdateDataAsync()
    {
        try
        {
            await _userSessionsRepository.TriggerSessionUpdatesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating data");
        }
    }
}