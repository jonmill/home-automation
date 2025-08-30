using System.Text.Json.Serialization;

namespace HomeAutomation.Models.Push;

/// <summary>
/// Represents the payload for a push notification.
/// </summary>
public sealed class PushPayload
{
    /// <summary>
    /// The title of the push notification.
    /// </summary>
    [JsonPropertyName("title")]
    public required string Title { get; set; }

    /// <summary>
    /// The message of the push notification.
    /// </summary>
    [JsonPropertyName("body")]
    public required string Message { get; set; }

    /// <summary>
    /// The tag for the push notification.
    /// </summary>
    [JsonPropertyName("tag")]
    public string Tag => Guid.NewGuid().ToString();
}