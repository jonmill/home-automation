using HomeAutomation.Database;
using HomeAutomation.Mqtt.Ingestors.Boards;
using HomeAutomation.Mqtt.Ingestors.Sensors;
using MQTTnet;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<MqttClientFactory>();
builder.Services.AddSingleton<MqttClientOptions>(sp =>
{
    IConfiguration config = sp.GetRequiredService<IConfiguration>();
    string brokerAddress = config.GetConnectionString("MqttBroker") ?? throw new KeyNotFoundException("MqttBroker connection string is not configured.");
    string mqttPassword = config.GetConnectionString("MqttPassword") ?? throw new KeyNotFoundException("MqttPassword is not configured.");
    return new MqttClientOptionsBuilder()
        .WithClientId("HomeAutomationMqttIngest")
        .WithTcpServer(brokerAddress)
        .WithCredentials("ha-mqtt", mqttPassword)
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
