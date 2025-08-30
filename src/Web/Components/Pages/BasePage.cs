using HomeAutomation.Web.Services;
using Microsoft.AspNetCore.Components;

namespace HomeAutomation.Web.Components.Pages;

public abstract class BasePage : ComponentBase, IDisposable
{
    [Inject]
    internal UserSession? UserSession { get; set; }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && (UserSession is not null))
        {
            UserSession.Dispose();
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}