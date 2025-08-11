using LinqToDB.Mapping;

namespace HomeAutomation.Models.Database;

/// <summary>
/// Represents a log output from a board.
/// </summary>
[Table(TableNames.LogEntries)]
public sealed class LogEntry
{
    /// <summary>
    /// The unique identifier for the log entry.
    /// </summary>
    [PrimaryKey, Identity]
    [Column("id"), NotNull]
    public long Id { get; set; }

    /// <summary>
    /// The timestamp when the log entry was created.
    /// </summary>
    [Column("timestamp"), NotNull]
    public required DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// The unique identifier for the board associated with the log entry.
    /// </summary>
    [Column("board_id"), NotNull]
    public required int BoardId { get; set; }

    /// <summary>
    /// The board associated with the log entry.
    /// </summary>
    [Association(ThisKey = nameof(BoardId), OtherKey = nameof(Board.Id))]
    public Board? Board { get; set; }

    /// <summary>
    /// The log level of the log entry.
    /// </summary>
    [Column("level"), NotNull]
    public required int Level { get; set; }

    /// <summary>
    /// The message of the log entry.
    /// </summary>
    [Column("message"), NotNull]
    public required string Message { get; set; }

    /// <summary>
    /// The exception associated with the log entry, if any.
    /// </summary>
    [Column("tag"), Nullable]
    public required string Tag { get; set; }
}