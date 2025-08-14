using LinqToDB.Mapping;

namespace HomeAutomation.Models.Database;

/// <summary>
/// Represents a heartbeat entry in the database.
/// </summary>
[Table(TableNames.Heartbeats)]
public sealed class Heartbeat
{
    /// <summary>
    /// The unique identifier for the heartbeat entry.
    /// </summary>
    [Column("Id"), PrimaryKey, Identity]
    public long Id { get; set; }

    /// <summary>
    /// The unique identifier for the board associated with the heartbeat entry.
    /// </summary>
    [Column("BoardSerialNumber"), NotNull]
    public required string BoardSerialNumber { get; set; }

    /// <summary>
    /// The board associated with the heartbeat entry.
    /// </summary>
    [Association(ThisKey = nameof(BoardSerialNumber), OtherKey = nameof(Board.SerialNumber))]
    public Board? Board { get; set; }

    /// <summary>
    /// The timestamp of the heartbeat entry.
    /// </summary>
    [Column("Timestamp"), NotNull]
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// The next expected heartbeat timestamp.
    /// </summary>
    [Column("NextExpectedHeartbeat"), NotNull]
    public DateTimeOffset NextExpectedHeartbeat { get; set; }
}