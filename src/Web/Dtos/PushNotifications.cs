namespace HomeAutomation.Web.Dtos;

public record SubscriptionDto(string endpoint, Keys keys);
public record Keys(string p256dh, string auth);