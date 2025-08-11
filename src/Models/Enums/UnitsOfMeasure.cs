namespace HomeAutomation.Models.Enums;

/// <summary>
/// Represents the various units of measurement used in the system.
/// </summary>
public enum UnitsOfMeasure : byte
{
    /// <summary>
    /// Represents the Celsius temperature scale.
    /// </summary>
    Celsius = 0,

    /// <summary>
    /// Represents the 0-100 percentage scale.
    /// </summary>
    Percentage = 1,

    /// <summary>
    /// Represents the light intensity measurement in lux.
    /// </summary>
    Lux = 2,

    /// <summary>
    /// Represents the air pressure measurement in pascals.
    /// </summary>
    Pascal = 3,

    /// <summary>
    /// Represents the electrical potential measurement in volts.
    /// </summary>
    Volt = 4,

    /// <summary>
    /// Represents a binary state (true/false).
    /// </summary>
    Boolean = 5,

    /// <summary>
    /// Represents the concentration of a substance in parts per billion.
    /// </summary>
    PartsPerBillion = 6,

    /// <summary>
    /// Represents the air quality index.
    /// </summary>
    AirQualityIndex = 7,
}