
using System.Text.Json.Serialization;

namespace HomeAutomation.Models.Mqtt;

/// <summary>
/// Represents the payload of a sensor output
/// </summary>
public sealed class DataPayload : ICloudEvent
{
    /// <inheritdoc />
    public required int BoardId { get; set; }

    /// <inheritdoc />
    public required DateTimeOffset Timestamp { get; set; }

    /// <inheritdoc />
    public required string ContentType { get; set; }

    /// <summary>
    /// The value coming out of a sensor on a board
    /// </summary>
    [JsonPropertyName("data_value")]
    public required string SensorValue { get; set; }

    /// <summary>
    /// The unique identifier for the sensor
    /// </summary>
    [JsonPropertyName("data_id")]
    public required string SensorId { get; set; }
}