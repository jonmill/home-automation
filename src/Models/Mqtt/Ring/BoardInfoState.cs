using System.Text.Json.Serialization;

namespace HomeAutomation.Models.Mqtt.Ring;

/// <summary>
/// Board information data coming from Ring 
/// </summary>
public sealed class BoardInfoState
{
    /// <summary>
    /// The battery level of the device (0-100).
    /// </summary>
    [JsonPropertyName("batteryLevel")]
    public required int BatteryLevel { get; init; }

    /// <summary>
    /// The battery status of the device.
    /// </summary>
    [JsonPropertyName("batteryStatus")]
    public required string BatteryStatus { get; init; }

    /// <summary>
    /// The communication status of the device.
    /// </summary>
    [JsonPropertyName("commStatus")]    
    public required string CommStatus { get; init; }

    /// <summary>
    /// The firmware status of the device.
    /// </summary>
    [JsonPropertyName("firmwareStatus")]
    public string FirmwareStatus { get; init; } = string.Empty;

    /// <summary>
    /// The last communication time of the device.
    /// </summary>
    [JsonPropertyName("lastCommTime")]
    public required DateTimeOffset LastCommTime { get; init; }

    /// <summary>
    /// The last update time of the device.
    /// </summary>
    [JsonPropertyName("lastUpdate")]
    public required DateTimeOffset LastUpdate { get; init; }

    /// <summary>
    /// The link quality of the device.
    /// </summary>
    [JsonPropertyName("linkQuality")]
    public string LinkQuality { get; init; } = string.Empty;

    /// <summary>
    /// The serial number of the device.
    /// </summary>
    [JsonPropertyName("serialNumber")]
    public string SerialNumber { get; init; } = string.Empty;

    /// <summary>
    /// The tamper status of the device.
    /// </summary>
    [JsonPropertyName("tamperStatus")]
    public required string TamperStatus { get; init; }
}