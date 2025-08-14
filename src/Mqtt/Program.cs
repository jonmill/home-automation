using HomeAutomation.Database;
using HomeAutomation.Mqtt.Ingestors.Boards;
using HomeAutomation.Mqtt.Ingestors.Sensors;
using HomeAutomation.Mqtt.Services;
using MQTTnet;

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
        .Build();
});
builder.Services.AddHostedService<SensorDataIngestor>()
                .AddHostedService<BoardHeartbeatIngestor>()
                .AddHostedService<InternalTemperatureIngestor>()
                .AddHostedService<LoggingIngestor>()
                .AddHostedService<MqttStateIngestor>()
                .AddHostedService<OtaStateIngestor>()
                .AddHostedService<PowerStateIngestor>();

builder.Services.AddHomeAutomationDatabase(builder.Configuration);

WebApplication app = builder.Build();

app.Services.MigrateDatabase();

app.Run();
