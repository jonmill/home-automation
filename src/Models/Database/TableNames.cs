namespace HomeAutomation.Models.Database;

/// <summary>
/// The name of the table that stores information about IoT boards.
/// </summary>
internal static class TableNames
{
    /// <summary>
    /// The name of the table that stores information about IoT boards.
    /// </summary>
    public const string Boards = "boards";

    /// <summary>
    /// The name of the table that stores information about sensors.
    /// </summary>
    public const string Sensors = "sensors";

    /// <summary>
    /// The name of the table that stores sensor values.
    /// </summary>
    public const string SensorValues = "sensorvalues";

    /// <summary>
    /// The name of the table that stores log entries.
    /// </summary>
    public const string LogEntries = "logentries";

    /// <summary>
    /// The name of the table that stores heartbeat information.
    /// </summary>
    public const string Heartbeats = "heartbeats";
}