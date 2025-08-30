using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeAutomation.PushExtensions;

public static class PushExtensions
{
    /// <summary>
    /// Adds the Home Automation push notification services.
    /// </summary>
    /// <param name="services">The apps service collection</param>
    /// <returns>Returns the updated service collection</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IServiceCollection AddPushNotifications(this IServiceCollection services, IConfiguration config)
    {
        services.AddPushServiceClient(options =>
        {
            string pubKey = config.GetValue<string>("Push:PublicKey") ?? throw new KeyNotFoundException("Push:PublicKey not found in configuration");
            string privateKey = config.GetValue<string>("Push:PrivateKey") ?? throw new KeyNotFoundException("Push:PrivateKey not found in configuration");
            options.AutoRetryAfter = true;
            options.DefaultTimeToLive = Convert.ToInt32(TimeSpan.FromDays(1).TotalSeconds);
            options.MaxRetriesAfter = 0;
            options.Subject = "mailto:security@framlux.io";
            options.PrivateKey = privateKey;
            options.PublicKey = pubKey;
        });

        services.AddSingleton<IPushNotifier, PushNotifier>();

        return services;
    }
}
