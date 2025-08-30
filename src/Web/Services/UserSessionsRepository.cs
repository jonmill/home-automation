using System.Collections.Concurrent;

namespace HomeAutomation.Web.Services;

internal sealed class UserSessionsRepository
{
    private readonly ConcurrentDictionary<Guid, WeakReference<UserSession>> _sessions = new();

    public UserSession CreateSession()
    {
        UserSession session = new(this);
        _sessions.TryAdd(session.SessionId, new WeakReference<UserSession>(session));
        return session;
    }

    public void RemoveSession(Guid sessionId) => _sessions.TryRemove(sessionId, out _);

    public async Task TriggerSessionUpdatesAsync()
    {
        foreach (var weakReference in _sessions.Values)
        {
            if (weakReference.TryGetTarget(out var session))
            {
                await session.NotifyDataUpdatedAsync();
            }
        }
    }
}