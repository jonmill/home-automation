using LinqToDB.Mapping;

namespace HomeAutomation.Models.Database;

/// <summary>
/// Represents the battery information for a specific board.
/// </summary>
[Table(TableNames.BoardBatteryInfo)]
public sealed class BoardBatteryInfo
{
    /// <summary>
    /// Gets or sets the unique identifier for the board.
    /// </summary>
    [Column("BoardId"), NotNull]
    public required int BoardId { get; set; }

    /// <summary>
    /// Gets or sets the board associated with this battery information.
    /// </summary>
    [Association(ThisKey = nameof(BoardId), OtherKey = nameof(Board.Id))]
    public Board? Board { get; set; }

    /// <summary>
    /// Gets or sets the battery level of the board.
    /// </summary>
    [Column("BatteryLevel"), NotNull]
    public required double BatteryLevel { get; set; }
    
    /// <summary>
    /// Gets or sets the timestamp of the last update to the battery information.
    /// </summary>
    [Column("LastUpdated"), NotNull]
    public required DateTimeOffset LastUpdated { get; set; }
}