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
    [Column("Id"), NotNull]
    public long Id { get; set; }

    /// <summary>
    /// The timestamp when the log entry was created.
    /// </summary>
    [Column("Timestamp"), NotNull]
    public required DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// The unique identifier for the board associated with the log entry.
    /// </summary>
    [Column("BoardSerialNumber"), NotNull]
    public required string BoardSerialNumber { get; set; }

    /// <summary>
    /// The board associated with the log entry.
    /// </summary>
    [Association(ThisKey = nameof(BoardSerialNumber), OtherKey = nameof(Board.SerialNumber))]
    public Board? Board { get; set; }

    /// <summary>
    /// The log level of the log entry.
    /// </summary>
    [Column("Level"), NotNull]
    public required int Level { get; set; }

    /// <summary>
    /// The message of the log entry.
    /// </summary>
    [Column("Message"), NotNull]
    public required string Message { get; set; }

    /// <summary>
    /// The exception associated with the log entry, if any.
    /// </summary>
    [Column("Tag"), Nullable]
    public required string Tag { get; set; }
}