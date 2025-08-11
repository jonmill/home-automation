using System.Text.Json.Serialization;

namespace HomeAutomation.Models.Mqtt;

/// <summary>
/// Represents a heartbeat event from a device, notifying that it is online
/// </summary>
public sealed class Heartbeat : ICloudEvent
{
    /// <inheritdoc />
    public required int BoardId { get; set; }

    /// <inheritdoc />
    public required DateTimeOffset Timestamp { get; set; }

    /// <inheritdoc />
    public required string ContentType { get; set; }

    /// <summary>
    /// The time in seconds until the next expected heartbeat
    /// </summary>
    [JsonPropertyName("data_next_heartbeat_in_seconds")]
    public required int NextExpectedHeartbeatInSeconds { get; set; }
}