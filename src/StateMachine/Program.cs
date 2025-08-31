using HomeAutomation.Database;
using HomeAutomation.MqttExtensions;
using HomeAutomation.PushExtensions;
using HomeAutomation.StateMachine.Models;
using HomeAutomation.StateMachine.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMqttIngestion(builder.Configuration);
builder.Services.AddHomeAutomationDatabase(builder.Configuration);
builder.Services.AddPushNotifications(builder.Configuration);
builder.Services.AddSingleton<HomeModel>();

builder.Services.AddHostedService<HomeStateMachine>();

var app = builder.Build();

app.Run();
