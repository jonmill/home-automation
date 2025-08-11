using System.Text.Json.Serialization;

namespace HomeAutomation.Models.Mqtt;

/// <summary>
/// Represents a log event from a device
/// </summary>
public sealed class LogEvent : ICloudEvent
{
    /// <inheritdoc />
    public required int BoardId { get; set; }

    /// <inheritdoc />
    public required DateTimeOffset Timestamp { get; set; }

    /// <inheritdoc />
    public required string ContentType { get; set; }

    /// <summary>
    /// The log message
    /// </summary>
    [JsonPropertyName("data_message")]
    public required string Message { get; set; }

    /// <summary>
    /// The log level
    /// </summary>
    [JsonPropertyName("data_level")]
    public required int Level { get; set; }

    /// <summary>
    /// The log tag
    /// </summary>
    [JsonPropertyName("data_tag")]
    public required string Tag { get; set; }
}