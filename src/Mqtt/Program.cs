using HomeAutomation.Database;
using HomeAutomation.Mqtt.Ingestors.Boards;
using HomeAutomation.Mqtt.Ingestors.Sensors;
using HomeAutomation.Mqtt.Services;
using MQTTnet;
using Polly;
using Polly.Retry;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IDatabaseCache, DatabaseCache>();
builder.Services.AddSingleton<MqttClientFactory>();
builder.Services.AddTransient<MqttClientOptions>(sp =>
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
builder.Services.AddHostedService<SensorDataIngestor>()
                .AddHostedService<BoardHeartbeatIngestor>()
                .AddHostedService<InternalTemperatureIngestor>()
                .AddHostedService<LoggingIngestor>()
                .AddHostedService<MqttStateIngestor>()
                .AddHostedService<OtaStateIngestor>()
                .AddHostedService<PowerStateIngestor>()
                .AddHostedService<RingBoardAttributesIngestor>()
                .AddHostedService<RingContactStateIngestor>();

builder.Services.AddResiliencePipeline("mqtt-pipeline", builder =>
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

builder.Services.AddHomeAutomationDatabase(builder.Configuration);

WebApplication app = builder.Build();

app.Services.MigrateDatabase();

app.Run();
