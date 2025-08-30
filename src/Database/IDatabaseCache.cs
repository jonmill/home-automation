using HomeAutomation.Models.Database;

namespace HomeAutomation.Database;

/// <summary>
/// Interface for caching database entities.
/// </summary>
public interface IDatabaseCache
{
    /// <summary>
    /// Gets a cached board entity by its serial number.
    /// </summary>
    /// <param name="serialNumber">The serial number of the board</param>
    /// <returns>Returns a Board if found; otherwise, returns null</returns>
    Task<Board?> GetBoardAsync(string serialNumber);

    /// <summary>
    /// Gets a cached sensor entity by its serial number.
    /// </summary>
    /// <param name="serialNumber">The serial number of the sensor</param>
    /// <returns>Returns a Sensor if found; otherwise, returns null</returns>
    Task<Sensor?> GetSensorAsync(string serialNumber);

    /// <summary>
    /// Gets all cached push subscription entities.
    /// </summary>
    /// <returns>Returns an enumeration of push subscriptions</returns>
    Task<IEnumerable<PushSubscription>> GetPushSubscriptionsAsync();

    /// <summary>
    /// Creates a new push subscription.
    /// </summary>
    /// <param name="endpoint">The push endpoint</param>
    /// <param name="p256dh">The p256dh key</param>
    /// <param name="auth">The auth key</param>
    /// <returns>Returns the created push subscription</returns>
    Task<PushSubscription> CreatePushSubscriptionAsync(string endpoint, string p256dh, string auth);

    /// <summary>
    /// Gets a cached push subscription entity by its endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint of the push subscription</param>
    /// <returns>Returns a PushSubscription if found; otherwise, returns null</returns>
    Task<PushSubscription?> GetPushSubscriptionAsync(string endpoint);

    /// <summary>
    /// Removes a stale push subscription from the cache and database.
    /// </summary>
    /// <param name="subscriptionEndpoint">The Subscription to remove</param>
    /// <returns>Returns an awaitable Task</returns>
    Task RemoveStalePushSubscriptionAsync(string subscriptionEndpoint);
}