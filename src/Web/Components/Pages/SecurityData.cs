using HomeAutomation.Models.Database;
using MudBlazor;

namespace HomeAutomation.Web.Components.Pages;

public record SecurityData
{
    public string SecurityStyle { get; set; }
    public string SecurityText { get; set; }

    public SecurityData(SensorValue? data)
    {
        if (bool.TryParse(data?.Value, out bool isClosed))
        {
            SecurityText = isClosed ? "Closed" : "Open";
            SecurityStyle = isClosed ? Colors.Green.Darken1 : Colors.Red.Darken1;
        }
        else
        {
            SecurityText = "Unknown";
            SecurityStyle = Colors.Gray.Darken1;
        }
    }
}