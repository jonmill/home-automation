using HomeAutomation.Database;
using HomeAutomation.MqttExtensions;
using HomeAutomation.Web.Components;
using HomeAutomation.Web.Dtos;
using HomeAutomation.Web.Services;
using LinqToDB;
using LinqToDB.Async;
using MudBlazor.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets(typeof(Program).Assembly, optional: true)
                     .AddJsonFile("appsettings.json", optional: false)
                     .AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add MQTT services
builder.Services.AddMqttIngestion(builder.Configuration);

// Add notifications
builder.Services.AddSingleton<UserSessionsRepository>();
builder.Services.AddScoped<UserSession>(sp =>
{
    UserSessionsRepository repository = sp.GetRequiredService<UserSessionsRepository>();
    return repository.CreateSession();
});

builder.Services.AddMudServices();
builder.Services.AddSingleton<ThemeService>();

builder.Services.AddHomeAutomationDatabase(builder.Configuration);

WebApplication app = builder.Build();

app.UseExceptionHandler("/Error", createScopeForErrors: true);

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapPost("/api/subscriptions", async (SubscriptionDto sub, HomeAutomationDb db) =>
{
    if (string.IsNullOrEmpty(sub.endpoint) ||
        string.IsNullOrEmpty(sub.keys?.p256dh) ||
        string.IsNullOrEmpty(sub.keys?.auth))
    {
        return Results.BadRequest("Invalid subscription data.");
    }

    HomeAutomation.Models.Database.PushSubscription? pushSubscription = await db.PushSubscriptions.SingleOrDefaultAsync(ps => ps.Endpoint == sub.endpoint.ToLower());
    if (pushSubscription is null)
    {
        pushSubscription = new()
        {
            Endpoint = sub.endpoint.ToLower(),
            P256dh = sub.keys.p256dh,
            Auth = sub.keys.auth,
        };
        await db.InsertAsync(pushSubscription);
    }

    return Results.Ok();
});

app.Run();
