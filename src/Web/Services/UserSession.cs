namespace HomeAutomation.Web.Services;

internal sealed class UserSession : IDisposable
{
    internal Guid SessionId { get; } = Guid.NewGuid();
    public delegate Task DataUpdatedHandler();
    public event DataUpdatedHandler? OnDataUpdated;

    private UserSessionsRepository _repository;
    private bool _hasUnregistered;

    internal UserSession(UserSessionsRepository repository)
    {
        _repository = repository;
        _hasUnregistered = false;
    }

    public async Task NotifyDataUpdatedAsync()
    {
        if (OnDataUpdated is not null)
        {
            await OnDataUpdated.Invoke();
        }
    }

    private void Dispose(bool disposing)
    {
        if (disposing && (_hasUnregistered == false))
        {
            _repository.RemoveSession(SessionId);
            _hasUnregistered = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}