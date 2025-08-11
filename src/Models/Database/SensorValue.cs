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
    [Column("board_id"), NotNull]
    public required int BoardId { get; set; }

    /// <summary>
    /// The unique identifier for the sensor.
    /// </summary>
    [PrimaryKey(Order = 2)]
    [Column("sensor_id"), NotNull]
    public required int SensorId { get; set; }

    /// <summary>
    /// The timestamp when the sensor value was recorded.
    /// </summary>
    [Column("timestamp"), NotNull]
    public required DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// The value recorded by the sensor.
    /// </summary>
    [Column("value"), NotNull]
    public required string Value { get; set; }

    /// <summary>
    /// The board associated with the sensor value.
    /// </summary>
    [Association(ThisKey = nameof(BoardId), OtherKey = nameof(Board.Id))]
    public Board? Board { get; set; }

    /// <summary>
    /// The sensor associated with the sensor value.
    /// </summary>
    [Association(ThisKey = nameof(SensorId), OtherKey = nameof(Sensor.Id))]
    public Sensor? Sensor { get; set; }
}