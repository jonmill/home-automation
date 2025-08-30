using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using Polly;
using Polly.Retry;

namespace HomeAutomation.MqttExtensions;

public static class MqttExtensions
{
    /// <summary>
    /// Adds MQTT ingestion services.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="config">The configuration provider</param>
    /// <returns>Returns the service collection</returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public static IServiceCollection AddMqttIngestion(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<MqttClientFactory>();
        services.AddTransient<MqttClientOptions>(sp =>
        {
            IConfiguration config = sp.GetRequiredService<IConfiguration>();
            string brokerAddress = config.GetConnectionString("MqttBroker") ?? throw new KeyNotFoundException("MqttBroker connection string is not configured.");
            string mqttPassword = config.GetConnectionString("MqttPassword") ?? throw new KeyNotFoundException("MqttPassword is not configured.");
            return new MqttClientOptionsBuilder()
                .WithTcpServer(brokerAddress, 1883)
                .WithCredentials("ha-mqtt", mqttPassword)
                .WithCleanSession(false)
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(5))
                .Build();

        });

        services.AddResiliencePipeline("mqtt-pipeline", builder =>
        {
            builder
            .AddRetry(new RetryStrategyOptions()
            {
                BackoffType = DelayBackoffType.Constant,
                Delay = TimeSpan.FromMilliseconds(500),
                MaxRetryAttempts = 5,
                UseJitter = false,
            })
            .AddTimeout(TimeSpan.FromSeconds(5));
        });

        return services;
    }
}