using HomeAutomation.Models.Database;
using LinqToDB;
using LinqToDB.Data;

namespace HomeAutomation.Database;

/// <summary>
/// Represents the database connection for the Home Automation application.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="HomeAutomationDb"/> class.
/// </remarks>
public sealed class HomeAutomationDb(DataOptions<HomeAutomationDb> options) : DataConnection(options.Options)
{
    /// <summary>
    /// Gets the table for IoT boards.
    /// </summary>
    public ITable<Board> Boards => this.GetTable<Board>();

    /// <summary>
    /// Gets the table for heartbeat entries.
    /// </summary>
    public ITable<Heartbeat> Heartbeats => this.GetTable<Heartbeat>();

    /// <summary>
    /// Gets the table for sensors.
    /// </summary>
    public ITable<Sensor> Sensors => this.GetTable<Sensor>();

    /// <summary>
    /// Gets the table for sensor values.
    /// </summary>
    public ITable<SensorValue> SensorValues => this.GetTable<SensorValue>();

    /// <summary>
    /// Gets the table for log entries.
    /// </summary>
    public ITable<LogEntry> LogEntries => this.GetTable<LogEntry>();
}