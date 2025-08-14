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
    [Column("Id"), NotNull]
    public int Id { get; set; }

    /// <summary>
    /// The unique number for the board.
    /// </summary>
    [Column("SerialNumber"), NotNull]
    public required string SerialNumber { get; set; }

    /// <summary>
    /// The name of the board.
    /// </summary>
    [Column("Name"), NotNull]
    public required string Name { get; set; }

    /// <summary>
    /// The date and time when the board was added.
    /// </summary>
    [Column("AddedAt"), NotNull]
    public required DateTimeOffset AddedAt { get; set; }

    /// <summary>
    /// Indicates whether the board is powered by a battery.
    /// </summary>
    [Column("OnBattery"), NotNull]
    public required bool OnBattery { get; set; }

    /// <summary>
    /// Indicates whether the board has been deleted.
    /// </summary>
    [Column("IsDeleted"), NotNull]
    public required bool IsDeleted { get; set; }

    /// <summary>
    /// Indicates when the board was deleted.
    /// </summary>
    [Column("DeletedOn")]
    public DateTimeOffset? DeletedOn { get; set; }

    /// <summary>
    /// The sensors connected to the board.
    /// </summary>
    [Association(ThisKey = nameof(Id), OtherKey = nameof(Sensor.BoardSerialNumber))]
    public List<Sensor> Sensors { get; set; } = [];
}