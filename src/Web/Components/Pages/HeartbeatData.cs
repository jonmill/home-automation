using HomeAutomation.Models.Database;
using HomeAutomation.Web.Components.Helpers;
using MudBlazor;

namespace HomeAutomation.Web.Components.Pages;

public record HeartbeatData
{
    public string SystemName { get; set; }
    public string HeartbeatStyle { get; set; }
    public string HeartbeatText { get; set; }
    public string BatteryText { get; set; }
    public bool HasBattery => !string.IsNullOrWhiteSpace(BatteryText);

    public HeartbeatData()
    {
        HeartbeatStyle = Colors.Gray.Darken1;
        HeartbeatText = "Unknown";
        SystemName = "Unknown";
        BatteryText = string.Empty;
    }

    public HeartbeatData(Board board, Heartbeat? heartbeat, BoardBatteryInfo? batteryInfo)
    {
        SystemName = board.Name;
        (HeartbeatText, HeartbeatStyle) = DateTimeHelpers.ToFriendlyStringWithTimeBasedColor(heartbeat?.Timestamp);
        BatteryText = $"{Math.Round(batteryInfo?.BatteryLevel ?? 0.0, 0)}%" ?? string.Empty;
    }
}