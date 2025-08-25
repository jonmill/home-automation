using HomeAutomation.Models.Database;
using HomeAutomation.Web.Components.Helpers;
using MudBlazor;

namespace HomeAutomation.Web.Components.Pages;

public record SecurityData
{
    public string SensorName { get; set; }
    public string SecurityStyle { get; set; }
    public string SecurityText { get; set; }
    public string LastMeasured { get; set; }

    public SecurityData(SensorValue? data, Sensor sensor)
    {
        SensorName = sensor.Name;
        if (bool.TryParse(data?.Value, out bool isOpen))
        {
            SecurityText = isOpen ? "Open" : "Closed";
            SecurityStyle = isOpen ? Colors.Red.Darken1 : Colors.Green.Darken1;
            LastMeasured = DateTimeHelpers.GetFriendlyTimeString(data?.Timestamp);
        }
        else
        {
            SecurityText = "Unknown";
            SecurityStyle = Colors.Gray.Darken1;
            LastMeasured = "No Data";
        }
    }
}