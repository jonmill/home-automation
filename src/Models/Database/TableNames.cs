namespace HomeAutomation.Models.Database;

/// <summary>
/// The name of the table that stores information about IoT boards.
/// </summary>
internal static class TableNames
{
    /// <summary>
    /// The name of the table that stores information about IoT boards.
    /// </summary>
    public const string Boards = "Boards";

    /// <summary>
    /// The name of the table that stores battery information for IoT boards.
    /// </summary>
    public const string BoardBatteryInfo = "BoardBatteryInfos";

    /// <summary>
    /// The name of the table that stores information about sensors.
    /// </summary>
    public const string Sensors = "Sensors";

    /// <summary>
    /// The name of the table that stores sensor values.
    /// </summary>
    public const string SensorValues = "SensorValues";

    /// <summary>
    /// The name of the table that stores log entries.
    /// </summary>
    public const string LogEntries = "LogEntries";

    /// <summary>
    /// The name of the table that stores heartbeat information.
    /// </summary>
    public const string Heartbeats = "Heartbeats";

    /// <summary>
    /// The name of the table that stores push subscription information.
    /// </summary>
    public const string PushSubscriptions = "PushSubscriptions";
}