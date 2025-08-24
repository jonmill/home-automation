using HomeAutomation.Models.Database;
using MudBlazor;

namespace HomeAutomation.Web.Components.Pages;

public record SecurityData
{
    public string SecurityStyle { get; set; }
    public string SecurityText { get; set; }

    public SecurityData(SensorValue? data)
    {
        if (bool.TryParse(data?.Value, out bool isOpen))
        {
            SecurityText = isOpen ? "Open" : "Closed";
            SecurityStyle = isOpen ? Colors.Red.Darken1 : Colors.Green.Darken1;
        }
        else
        {
            SecurityText = "Unknown";
            SecurityStyle = Colors.Gray.Darken1;
        }
    }
}