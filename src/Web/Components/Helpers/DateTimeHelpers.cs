using MudBlazor;

namespace HomeAutomation.Web.Components.Helpers;

public static class DateTimeHelpers
{
    public static (string text, string color) ToFriendlyStringWithTimeBasedColor(DateTimeOffset? dateTime, DateTimeOffset? nextTime)
    {
        string text = GetFriendlyTimeString(dateTime);
        string color = GetFriendlyDateColorByTime(dateTime, nextTime);
        return (text, color);
    }

    public static string GetFriendlyTimeString(DateTimeOffset? dateTime)
    {
        if (dateTime is null)
        {
            return "Unknown";
        }

        TimeSpan ago = DateTimeOffset.Now - dateTime.Value;
        int minutesSince = (int)ago.TotalMinutes;
        int hoursSince = (int)ago.TotalHours;
        int daysSince = (int)ago.TotalDays;
        if (daysSince > 0)
        {
            return $"{daysSince} day{(daysSince > 1 ? "s" : "")} ago";
        }
        else if (hoursSince > 0)
        {
            return $"{hoursSince} hour{(hoursSince > 1 ? "s" : "")} ago";
        }
        else if (minutesSince >= 30)
        {
            return $"{minutesSince} minute{(minutesSince > 1 ? "s" : "")} ago";
        }
        else if (minutesSince >= 2)
        {
            return $"{minutesSince} minutes ago";
        }
        else if (minutesSince == 1)
        {
            return $"{minutesSince} minute ago";
        }
        else if (minutesSince < 1)
        {
            return "Just now";
        }
        else
        {
            return "Unknown";
        }
    }

    private static string GetFriendlyDateColorByTime(DateTimeOffset? dateTime, DateTimeOffset? nextTime)
    {
        if (dateTime is null)
        {
            return Colors.Gray.Darken1;
        }

        if (nextTime.HasValue == false)
        {
            nextTime = dateTime?.AddMinutes(1) ?? DateTimeOffset.Now.AddMinutes(1);
        }

        if (DateTimeOffset.Now > nextTime.Value)
        {
            TimeSpan overdue = DateTimeOffset.Now - nextTime.Value;
            if (overdue.TotalDays >= 1)
            {
                return Colors.Red.Darken4;
            }
            else if (overdue.TotalHours >= 1)
            {
                return Colors.Orange.Darken4;
            }
            else if (overdue.TotalMinutes >= 30)
            {
                return Colors.Yellow.Darken4;
            }
            else if (overdue.TotalMinutes >= 2)
            {
                return Colors.Yellow.Lighten1;
            }
            else if (overdue.TotalMinutes == 1)
            {
                return Colors.Green.Lighten1;
            }
            else if (overdue.TotalMinutes < 1)
            {
                return Colors.Green.Darken1;
            }
            else
            {
                return Colors.Gray.Darken1;
            }
        }
        else
        {
            return Colors.Green.Darken1;
        }
    }
}
