namespace HomeAutomation.Models.Enums;

/// <summary>
/// Represents the various types of sensors available.
/// </summary>
public enum SensorTypes : byte
{
    /// <summary>
    /// A temperature measurement
    /// </summary>
    Temperature = 0,

    /// <summary>
    /// An air moisture measurement
    /// </summary>
    Humidity = 1,

    /// <summary>
    /// A light intensity measurement
    /// </summary>
    Light = 2,

    /// <summary>
    /// A motion detection measurement
    /// </summary>
    Motion = 3,

    /// <summary>
    /// An air pressure measurement
    /// </summary>
    Pressure = 4,

    /// <summary>
    /// A voltage measurement
    /// </summary>
    Voltage = 5,

    /// <summary>
    /// A contact measurement
    /// </summary>
    Contact = 6,

    /// <summary>
    /// An organic compound measurement
    /// </summary>
    OrganicCompound = 7,

    /// <summary>
    /// An air quality measurement
    /// </summary>
    AirQuality = 8,

    /// <summary>
    /// An air quality confidence measurement
    /// </summary>
    AirQualityConfidence = 9,
}