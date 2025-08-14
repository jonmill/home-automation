using HomeAutomation.Models.Enums;
using LinqToDB.Mapping;

namespace HomeAutomation.Models.Database;

/// <summary>
/// Represents a sensor connected to an IoT board.
/// </summary>
[Table(TableNames.Sensors)]
public sealed class Sensor
{
    /// <summary>
    /// The unique identifier for the sensor.
    /// </summary>
    [PrimaryKey, Identity]
    [Column("Id"), NotNull]
    public int Id { get; set; }

    /// <summary>
    /// The unique number for the sensor.
    /// </summary>
    [Column("SerialNumber"), NotNull]
    public required string SerialNumber { get; set; }

    /// <summary>
    /// The name of the sensor.
    /// </summary>
    [Column("Name"), NotNull]
    public required string Name { get; set; }

    /// <summary>
    /// The measurement type of the sensor.
    /// </summary>
    [Column("Type"), NotNull]
    public required SensorTypes Type { get; set; }

    /// <summary>
    /// The unit of measurement for the sensor data.
    /// </summary>
    [Column("Unit"), NotNull]
    public required UnitsOfMeasure Unit { get; set; }

    /// <summary>
    /// The unique identifier for the board the sensor is connected to.
    /// </summary>
    [Column("BoardSerialNumber"), NotNull]
    public int BoardSerialNumber { get; set; }

    /// <summary>
    /// The board the sensor is connected to.
    /// </summary>
    [Association(ThisKey = nameof(BoardSerialNumber), OtherKey = nameof(Board.SerialNumber))]
    public Board? Board { get; set; }

    /// <summary>
    /// The date and time when the sensor was added.
    /// </summary>
    [Column("AddedAt"), NotNull]
    public required DateTimeOffset AddedAt { get; set; }

    /// <summary>
    /// Indicates whether the sensor is deleted.
    /// </summary>
    [Column("IsDeleted"), NotNull]
    public required bool IsDeleted { get; set; }

    /// <summary>
    /// The date and time when the sensor was deleted.
    /// </summary>
    [Column("DeletedOn")]
    public DateTimeOffset? DeletedOn { get; set; }
}