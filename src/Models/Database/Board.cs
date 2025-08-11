using LinqToDB.Mapping;

namespace HomeAutomation.Models.Database;

/// <summary>
/// Represents an IoT board
/// </summary>
[Table(TableNames.Boards)]
public sealed class Board
{
    /// <summary>
    /// The unique identifier for the board.
    /// </summary>
    [PrimaryKey, Identity]
    [Column("id"), NotNull]
    public int Id { get; set; }

    /// <summary>
    /// The name of the board.
    /// </summary>
    [Column("name"), NotNull]
    public required string Name { get; set; }

    /// <summary>
    /// The date and time when the board was added.
    /// </summary>
    [Column("added_at"), NotNull]
    public required DateTimeOffset AddedAt { get; set; }

    /// <summary>
    /// Indicates whether the board is powered by a battery.
    /// </summary>
    [Column("on_battery"), NotNull]
    public required bool OnBattery { get; set; }

    /// <summary>
    /// Indicates whether the board has been deleted.
    /// </summary>
    [Column("is_deleted"), NotNull]
    public required bool IsDeleted { get; set; }

    /// <summary>
    /// Indicates when the board was deleted.
    /// </summary>
    [Column("deleted_on")]
    public DateTimeOffset? DeletedOn { get; set; }

    /// <summary>
    /// The sensors connected to the board.
    /// </summary>
    [Association(ThisKey = nameof(Id), OtherKey = nameof(Sensor.BoardId))]
    public List<Sensor> Sensors { get; set; } = [];
}