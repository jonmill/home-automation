namespace HomeAutomation.Models.Mqtt.Ring;

/// <summary>
/// Board information data coming from Ring 
/// </summary>
public sealed class BoardInfoState
{
    /// <summary>
    /// The battery level of the device (0-100).
    /// </summary>
    public required int BatteryLevel { get; init; }

    /// <summary>
    /// The battery status of the device.
    /// </summary>
    public required string BatteryStatus { get; init; }

    /// <summary>
    /// The communication status of the device.
    /// </summary>
    public required string CommStatus { get; init; }

    /// <summary>
    /// The firmware status of the device.
    /// </summary>
    public required string FirmwareStatus { get; init; }

    /// <summary>
    /// The last communication time of the device.
    /// </summary>
    public required DateTimeOffset LastCommTime { get; init; }

    /// <summary>
    /// The last update time of the device.
    /// </summary>
    public required DateTimeOffset LastUpdate { get; init; }

    /// <summary>
    /// The link quality of the device.
    /// </summary>
    public required string LinkQuality { get; init; }

    /// <summary>
    /// The serial number of the device.
    /// </summary>
    public required string SerialNumber { get; init; }

    /// <summary>
    /// The tamper status of the device.
    /// </summary>
    public required string TamperStatus { get; init; }
}