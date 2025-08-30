using HomeAutomation.Database;
using HomeAutomation.Models.Push;
using Lib.Net.Http.WebPush;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace HomeAutomation.PushExtensions;

public sealed class PushNotifier : IPushNotifier
{
    private readonly PushServiceClient _client;
    private readonly IDatabaseCache _databaseCache;
    private readonly ILogger<PushNotifier> _logger;

    public PushNotifier(PushServiceClient client, IDatabaseCache database, ILogger<PushNotifier> logger)
    {
        _client = client;
        _databaseCache = database;
        _logger = logger;
    }

    public async Task NotifyAsync(string title, string message)
    {
        PushPayload payload = new()
        {
            Title = title,
            Message = message
        };
        string payloadString = JsonSerializer.Serialize(payload);
        IEnumerable<Models.Database.PushSubscription> subscriptions = await _databaseCache.GetPushSubscriptionsAsync();
        foreach (Models.Database.PushSubscription subscription in subscriptions)
        {
            PushSubscription pushSubscription = new()
            {
                Endpoint = subscription.Endpoint,
            };
            pushSubscription.SetKey(PushEncryptionKeyName.P256DH, subscription.P256dh);
            pushSubscription.SetKey(PushEncryptionKeyName.Auth, subscription.Auth);

            try
            {
                PushMessage msg = new(payloadString);
                await _client.RequestPushMessageDeliveryAsync(pushSubscription, msg);
            }
            catch (Exception ex)
            {
                await OnPushErrorAsync(ex, pushSubscription);
            }
        }
    }

    public async Task NotifySpecificAsync(string endpoint, string p256dh, string auth, string title, string message)
    {
        PushPayload payload = new()
        {
            Title = title,
            Message = message
        };
        string payloadString = JsonSerializer.Serialize(payload);
        PushSubscription pushSubscription = new()
        {
            Endpoint = endpoint,
        };
        pushSubscription.SetKey(PushEncryptionKeyName.P256DH, p256dh);
        pushSubscription.SetKey(PushEncryptionKeyName.Auth, auth);

        try
        {
            PushMessage msg = new(payloadString);
            await _client.RequestPushMessageDeliveryAsync(pushSubscription, msg);
        }
        catch (Exception ex)
        {
            await OnPushErrorAsync(ex, pushSubscription);
        }
    }

    private async Task OnPushErrorAsync(Exception exception, PushSubscription subscription)
    {
        PushServiceClientException? pushServiceClientException = exception as PushServiceClientException;
        if (pushServiceClientException is null)
        {
            _logger.LogError(exception, "Unknown push notification error occurred.");
        }
        else
        {
            if ((pushServiceClientException.StatusCode == HttpStatusCode.NotFound) || (pushServiceClientException.StatusCode == HttpStatusCode.Gone))
            {
                await _databaseCache.RemoveStalePushSubscriptionAsync(subscription.Endpoint);
                _logger.LogInformation("Subscription {SubEndpoint} has expired or is no longer valid and has been removed.", subscription.Endpoint);
            }
        }
    }
}