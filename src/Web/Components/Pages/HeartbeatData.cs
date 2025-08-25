using HomeAutomation.Models.Database;
using HomeAutomation.Web.Components.Helpers;
using MudBlazor;

namespace HomeAutomation.Web.Components.Pages;

public record HeartbeatData
{
    public string SystemName { get; set; }
    public string HeartbeatStyle { get; set; }
    public string HeartbeatText { get; set; }

    public HeartbeatData()
    {
        HeartbeatStyle = Colors.Gray.Darken1;
        HeartbeatText = "Unknown";
        SystemName = "Unknown";
    }

    public HeartbeatData(Heartbeat? heartbeat)
    {
        SystemName = heartbeat?.Board?.Name ?? "Unknown";
        (HeartbeatText, HeartbeatStyle) = DateTimeHelpers.ToFriendlyStringWithTimeBasedColor(heartbeat?.Timestamp);
    }
}