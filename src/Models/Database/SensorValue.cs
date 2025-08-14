using LinqToDB.Mapping;

namespace HomeAutomation.Models.Database;

/// <summary>
/// Represents a value recorded by a sensor.
/// </summary>
[Table(TableNames.SensorValues)]
public sealed class SensorValue
{
    /// <summary>
    /// The unique identifier for the board.
    /// </summary>
    [PrimaryKey(Order = 1)]
    [Column("BoardSerialNumber"), NotNull]
    public required string BoardSerialNumber { get; set; }

    /// <summary>
    /// The unique identifier for the sensor.
    /// </summary>
    [PrimaryKey(Order = 2)]
    [Column("SensorSerialNumber"), NotNull]
    public required string SensorSerialNumber { get; set; }

    /// <summary>
    /// The timestamp when the sensor value was recorded.
    /// </summary>
    [Column("Timestamp"), NotNull]
    public required DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// The value recorded by the sensor.
    /// </summary>
    [Column("Value"), NotNull]
    public required string Value { get; set; }

    /// <summary>
    /// The board associated with the sensor value.
    /// </summary>
    [Association(ThisKey = nameof(BoardSerialNumber), OtherKey = nameof(Board.SerialNumber))]
    public Board? Board { get; set; }

    /// <summary>
    /// The sensor associated with the sensor value.
    /// </summary>
    [Association(ThisKey = nameof(SensorSerialNumber), OtherKey = nameof(Sensor.SerialNumber))]
    public Sensor? Sensor { get; set; }
}