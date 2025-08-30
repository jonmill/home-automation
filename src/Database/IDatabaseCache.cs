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
}