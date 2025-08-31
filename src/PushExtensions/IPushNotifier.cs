namespace HomeAutomation.PushExtensions;

/// <summary>
/// Interface for sending push notifications.
/// </summary>
public interface IPushNotifier
{
    /// <summary>
    /// Sends a push notification.
    /// </summary>
    /// <param name="title">The title of the notification</param>
    /// <param name="message">The message of the notification</param>
    /// <param name="highPriority">Whether the notification is high priority</param>
    /// <returns>Returns an awaitable Task</returns>
    Task NotifyAsync(string title, string message, bool highPriority);

    /// <summary>
    /// Sends a push notification to a specific endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint to send to</param>
    /// <param name="p256dh">The user's public VAPID key</param>
    /// <param name="auth">The user's authentication secret</param>
    /// <param name="title">The title of the notification</param>
    /// <param name="message">The message of the notification</param>
    /// <returns>Returns an awaitable Task</returns>
    Task NotifySpecificAsync(string endpoint, string p256dh, string auth, string title, string message);
}