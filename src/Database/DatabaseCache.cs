using HomeAutomation.Models.Database;
using LinqToDB;
using LinqToDB.Async;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace HomeAutomation.Database;

/// <summary>
/// Caches database entities in memory for faster access.
/// </summary>
public sealed class DatabaseCache : IDatabaseCache
{
    private readonly ConcurrentDictionary<string, Board> _boardCache;
    private readonly ConcurrentDictionary<string, Sensor> _sensorCache;
    private readonly ConcurrentDictionary<string, PushSubscription> _pushSubscriptionCache;
    private readonly IServiceScopeFactory _serviceScopeFactory;


    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseCache"/> class.
    /// </summary>
    /// <param name="serviceScopeFactory">The scope factory for scoped services</param>
    public DatabaseCache(IServiceScopeFactory serviceScopeFactory)
    {
        _boardCache = [];
        _sensorCache = [];
        _pushSubscriptionCache = [];
        _serviceScopeFactory = serviceScopeFactory;
    }

    /// <inheritdoc />
    public async Task<Board?> GetBoardAsync(string serialNumber)
    {
        if (_boardCache.TryGetValue(serialNumber, out Board? board))
        {
            return board;
        }

        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        using HomeAutomationDb dbContext = scope.ServiceProvider.GetRequiredService<HomeAutomationDb>();
        board = await dbContext.Boards.SingleOrDefaultAsync(b => b.SerialNumber == serialNumber);

        if (board is not null)
        {
            _boardCache.TryAdd(serialNumber, board);
        }

        return board;
    }

    /// <inheritdoc />
    public async Task<Sensor?> GetSensorAsync(string serialNumber)
    {
        if (_sensorCache.TryGetValue(serialNumber, out Sensor? sensor))
        {
            return sensor;
        }

        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        using HomeAutomationDb dbContext = scope.ServiceProvider.GetRequiredService<HomeAutomationDb>();
        sensor = await dbContext.Sensors.SingleOrDefaultAsync(s => s.SerialNumber == serialNumber);

        if (sensor is not null)
        {
            _sensorCache.TryAdd(serialNumber, sensor);
        }

        return sensor;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PushSubscription>> GetPushSubscriptionsAsync()
    {
        if (_pushSubscriptionCache.IsEmpty)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            using HomeAutomationDb dbContext = scope.ServiceProvider.GetRequiredService<HomeAutomationDb>();
            await foreach (PushSubscription subscription in dbContext.PushSubscriptions.AsAsyncEnumerable())
            {
                _pushSubscriptionCache.TryAdd(subscription.Endpoint, subscription);
            }
        }

        PushSubscription[] subscriptions = new PushSubscription[_pushSubscriptionCache.Count];
        _pushSubscriptionCache.Values.CopyTo(subscriptions, 0);

        return subscriptions;
    }

    /// <inheritdoc />
    public async Task RemoveStalePushSubscriptionAsync(string subscriptionEndpoint)
    {
        if (string.IsNullOrEmpty(subscriptionEndpoint))
        {
            return;
        }

        string endpoint = subscriptionEndpoint.ToLower();
        _pushSubscriptionCache.TryRemove(endpoint, out _);
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        using HomeAutomationDb dbContext = scope.ServiceProvider.GetRequiredService<HomeAutomationDb>();
        await dbContext.PushSubscriptions.Where(s => s.Endpoint == endpoint).DeleteAsync();
    }

    /// <inheritdoc />
    public async Task<PushSubscription> CreatePushSubscriptionAsync(string endpoint, string p256dh, string auth)
    {
        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(p256dh) || string.IsNullOrEmpty(auth))
        {
            throw new ArgumentException("Endpoint, p256dh, and auth must be provided to create a push subscription.");
        }

        string endpointNormalized = endpoint.ToLower();
        PushSubscription subscription = new()
        {
            Endpoint = endpointNormalized,
            P256dh = p256dh,
            Auth = auth,
        };

        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        using HomeAutomationDb dbContext = scope.ServiceProvider.GetRequiredService<HomeAutomationDb>();
        await dbContext.InsertAsync(subscription);
        _pushSubscriptionCache.TryAdd(endpointNormalized, subscription);

        return subscription;
    }

    /// <inheritdoc />
    public async Task<PushSubscription?> GetPushSubscriptionAsync(string endpoint)
    {
        string normalizedEndpoint = endpoint.ToLower();
        if (_pushSubscriptionCache.TryGetValue(normalizedEndpoint, out PushSubscription? subscription))
        {
            return subscription;
        }

        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        using HomeAutomationDb dbContext = scope.ServiceProvider.GetRequiredService<HomeAutomationDb>();
        subscription = await dbContext.PushSubscriptions.SingleOrDefaultAsync(s => s.Endpoint == normalizedEndpoint);

        if (subscription is not null)
        {
            _pushSubscriptionCache.TryAdd(normalizedEndpoint, subscription);
        }

        return subscription;
    }
}