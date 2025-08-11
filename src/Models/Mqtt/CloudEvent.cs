using System.Text.Json.Serialization;

namespace HomeAutomation.Models.Mqtt;

/// <summary>
/// Base event from IoT devices
/// </summary>
public interface ICloudEvent
{
    /// <summary>
    /// The ID of the board that generated the event
    /// </summary>
    [JsonPropertyName("source")]
    public int BoardId { get; set; }

    /// <summary>
    /// The time the event was generated
    /// </summary>
    [JsonPropertyName("time")]
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// The content type of the event, should be "application/json"
    /// </summary>
    [JsonPropertyName("content_type")]
    public string ContentType { get; set; }
}