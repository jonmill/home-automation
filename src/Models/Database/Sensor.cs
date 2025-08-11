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
    [Column("id"), NotNull]
    public int Id { get; set; }

    /// <summary>
    /// The name of the sensor.
    /// </summary>
    [Column("name"), NotNull]
    public required string Name { get; set; }

    /// <summary>
    /// The measurement type of the sensor.
    /// </summary>
    [Column("type"), NotNull]
    public required SensorTypes Type { get; set; }

    /// <summary>
    /// The unit of measurement for the sensor data.
    /// </summary>
    [Column("unit"), NotNull]
    public required UnitsOfMeasure Unit { get; set; }

    /// <summary>
    /// The unique identifier for the board the sensor is connected to.
    /// </summary>
    [Column("board_id"), NotNull]
    public int BoardId { get; set; }

    /// <summary>
    /// The board the sensor is connected to.
    /// </summary>
    [Association(ThisKey = nameof(BoardId), OtherKey = nameof(Board.Id))]
    public Board? Board { get; set; }

    /// <summary>
    /// The date and time when the sensor was added.
    /// </summary>
    [Column("added_at"), NotNull]
    public required DateTimeOffset AddedAt { get; set; }

    /// <summary>
    /// Indicates whether the sensor is deleted.
    /// </summary>
    [Column("is_deleted"), NotNull]
    public required bool IsDeleted { get; set; }

    /// <summary>
    /// The date and time when the sensor was deleted.
    /// </summary>
    [Column("deleted_on")]
    public DateTimeOffset? DeletedOn { get; set; }
}