using HomeAutomation.Models.Database;
using LinqToDB.Async;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace HomeAutomation.Database;

/// <summary>
/// Caches database entities in memory for faster access.
/// </summary>
public sealed class DatabaseCache : IDatabaseCache
{
    private readonly ConcurrentDictionary<string, Board> _boardCache = [];
    private readonly ConcurrentDictionary<string, Sensor> _sensorCache = [];
    private readonly IServiceScopeFactory _serviceScopeFactory;


    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseCache"/> class.
    /// </summary>
    /// <param name="serviceScopeFactory">The scope factory for scoped services</param>
    public DatabaseCache(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    /// <inheritdoc />
    public async Task<Board?> GetBoardAsync(string serialNumber)
    {
        if (_boardCache.TryGetValue(serialNumber, out Board? board))
        {
            return board;
        }

        using (IServiceScope scope = _serviceScopeFactory.CreateScope())
        {
            HomeAutomationDb dbContext = scope.ServiceProvider.GetRequiredService<HomeAutomationDb>();
            board = await dbContext.Boards.SingleOrDefaultAsync(b => b.SerialNumber == serialNumber);
        }

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

        using (IServiceScope scope = _serviceScopeFactory.CreateScope())
        {
            HomeAutomationDb dbContext = scope.ServiceProvider.GetRequiredService<HomeAutomationDb>();
            sensor = await dbContext.Sensors.SingleOrDefaultAsync(s => s.SerialNumber == serialNumber);
        }

        if (sensor is not null)
        {
            _sensorCache.TryAdd(serialNumber, sensor);
        }

        return sensor;
    }
}