using LinqToDB.Mapping;

namespace HomeAutomation.Models.Database;

/// <summary>
/// Represents a push subscription for a user.
/// </summary>
[Table(TableNames.PushSubscriptions)]
public sealed class PushSubscription
{
    /// <summary>
    /// The endpoint URL for the push subscription.
    /// </summary>
    [PrimaryKey]
    [Column("Endpoint"), NotNull]
    public required string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// The P256h key for this subscription
    /// </summary>
    [Column("P256dh"), NotNull]
    public required string P256dh { get; set; } = string.Empty;

    /// <summary>
    /// The Auth key for this subscription
    /// </summary>
    [Column("Auth"), NotNull]
    public required string Auth { get; set; } = string.Empty;
}