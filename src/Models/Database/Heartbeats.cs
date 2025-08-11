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
    [Column("id"), PrimaryKey, Identity]
    public long Id { get; set; }

    /// <summary>
    /// The unique identifier for the board associated with the heartbeat entry.
    /// </summary>
    [Column("board_id"), NotNull]
    public int BoardId { get; set; }

    /// <summary>
    /// The board associated with the heartbeat entry.
    /// </summary>
    [Association(ThisKey = nameof(BoardId), OtherKey = nameof(Board.Id))]
    public Board? Board { get; set; }

    /// <summary>
    /// The timestamp of the heartbeat entry.
    /// </summary>
    [Column("timestamp"), NotNull]
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// The next expected heartbeat timestamp.
    /// </summary>
    [Column("next_expected_heartbeat"), NotNull]
    public DateTimeOffset NextExpectedHeartbeat { get; set; }
}