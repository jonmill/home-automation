using HomeAutomation.Database;
using HomeAutomation.Mqtt.Ingestors.Boards;
using HomeAutomation.Mqtt.Ingestors.Sensors;
using HomeAutomation.MqttExtensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddMqttIngestion(builder.Configuration);
builder.Services.AddHostedService<SensorDataIngestor>()
                .AddHostedService<BoardHeartbeatIngestor>()
                .AddHostedService<InternalTemperatureIngestor>()
                .AddHostedService<LoggingIngestor>()
                .AddHostedService<MqttStateIngestor>()
                .AddHostedService<OtaStateIngestor>()
                .AddHostedService<PowerStateIngestor>()
                .AddHostedService<RingBoardAttributesIngestor>()
                .AddHostedService<RingContactStateIngestor>();

builder.Services.AddHomeAutomationDatabase(builder.Configuration);

WebApplication app = builder.Build();

app.Services.MigrateDatabase();

app.Run();
