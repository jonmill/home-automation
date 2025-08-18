using HomeAutomation.Database;
using HomeAutomation.Models.Database;
using HomeAutomation.Models.Mqtt.Ring;
using HomeAutomation.Mqtt.Services;
using LinqToDB;
using MQTTnet;
using System.Text.Json;

namespace HomeAutomation.Mqtt.Ingestors.Sensors;

internal sealed class RingBoardAttributesIngestor : IngestBase
{
    private readonly IDatabaseCache _databaseCache;

    public RingBoardAttributesIngestor(
        MqttClientOptions options,
        MqttClientFactory clientFactory,
        IServiceScopeFactory serviceScopeFactory,
        IDatabaseCache databaseCache,
        ILogger<RingBoardAttributesIngestor> logger)
        : base("ring/+/alarm/+/info/state", "Ha-RingBoardAttributesIngest", options, clientFactory, serviceScopeFactory, logger)
    {
        _databaseCache = databaseCache;
    }

    protected override async Task OnMessageReceived(string payload, MqttApplicationMessageReceivedEventArgs e)
    {
        string[] parts = e.ApplicationMessage.Topic.Split('/');
        if (parts.Length < 6)
        {
            _logger.LogWarning("Invalid topic format for Ring Attributes State: {Topic}", e.ApplicationMessage.Topic);
            return;
        }

        BoardInfoState? boardState;

        try
        {
            boardState = JsonSerializer.Deserialize<BoardInfoState>(payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize Ring Board Attributes payload: {Payload}", payload);
            return;
        }

        if (boardState is null)
        {
            _logger.LogError("Deserialized Ring Board Attributes is null for payload: {Payload}", payload);
            return;
        }

        Board? board = await _databaseCache.GetBoardAsync(boardState.SerialNumber);
        if (board is null)
        {
            _logger.LogWarning("Board with serial number {SerialNumber} not found in database.", boardState.SerialNumber);
            return;
        }

        _logger.LogInformation("Received Ring Board Attributes for {SerialNumber}: {BatteryLevel}", boardState.SerialNumber, boardState.BatteryLevel);

        try
        {
            _logger.LogInformation("Inserting heartbeat for Ring Board {SerialNumber}", boardState.SerialNumber);
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            using HomeAutomationDb db = scope.ServiceProvider.GetRequiredService<HomeAutomationDb>();
            await db.InsertAsync<Heartbeat>(new Heartbeat()
            {
                BoardSerialNumber = boardState.SerialNumber,
                NextExpectedHeartbeat = boardState.LastUpdate.AddHours(6), // Ring comms every 4-6 hours
                Timestamp = boardState.LastUpdate,
            });
            _logger.LogInformation("Successfully inserted heartbeat for Ring Board {SerialNumber}", boardState.SerialNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to insert heartbeat for Ring Board {SerialNumber}", boardState.SerialNumber);
            return;
        }
    }
}