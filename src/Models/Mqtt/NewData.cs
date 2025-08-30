namespace HomeAutomation.Models.Mqtt;

/// <summary>
/// Represents a notification about new data coming in
/// </summary>
public sealed class NewData
{
    /// <summary>
    /// Gets or sets the type of event that this notification is for.
    /// </summary>
    public NewDataEventTypes EventType { get; set; }
}