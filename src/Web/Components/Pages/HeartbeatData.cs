using HomeAutomation.Models.Database;
using MudBlazor;

namespace HomeAutomation.Web.Components.Pages;

public record HeartbeatData
{
    public string HeartbeatStyle { get; set; }
    public string HeartbeatText { get; set; }

    public HeartbeatData()
    {
        HeartbeatStyle = Colors.Gray.Darken1;
        HeartbeatText = "Unknown";
    }

    public HeartbeatData(Heartbeat? heartbeat)
    {
        TimeSpan ago = DateTimeOffset.Now - (heartbeat?.Timestamp ?? DateTimeOffset.MinValue);
        int minutesSince = (int)ago.TotalMinutes;
        int hoursSince = (int)ago.TotalHours;
        int daysSince = (int)ago.TotalDays;
        if (daysSince > 0)
        {
            HeartbeatText = $"{daysSince} day{(daysSince > 1 ? "s" : "")} ago";
            HeartbeatStyle = Colors.Red.Darken4;
        }
        else if (hoursSince > 0)
        {
            HeartbeatText = $"{hoursSince} hour{(hoursSince > 1 ? "s" : "")} ago";
            HeartbeatStyle = Colors.Orange.Darken4;
        }
        else if (minutesSince >= 30)
        {
            HeartbeatText = $"{minutesSince} minute{(minutesSince > 1 ? "s" : "")} ago";
            HeartbeatStyle = Colors.Yellow.Darken4;
        }
        else if (minutesSince >= 2)
        {
            HeartbeatText = $"{minutesSince} minutes ago";
            HeartbeatStyle = Colors.Yellow.Lighten1;
        }
        else if (minutesSince == 1)
        {
            HeartbeatText = $"{minutesSince} minute ago";
            HeartbeatStyle = Colors.Green.Darken1;
        }
        else if (minutesSince < 1)
        {
            HeartbeatText = "Just now";
            HeartbeatStyle = Colors.Green.Darken1;
        }
        else
        {
            HeartbeatText = "Unknown";
            HeartbeatStyle = Colors.Gray.Darken1;
        }
    }
}