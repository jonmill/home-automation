using HomeAutomation.Database;
using HomeAutomation.Models.Database;
using HomeAutomation.Mqtt.Services;
using LinqToDB;
using MQTTnet;

namespace HomeAutomation.Mqtt.Ingestors.Sensors;

internal sealed class RingContactStateIngestor : IngestBase
{
    private readonly IDatabaseCache _databaseCache;

    public RingContactStateIngestor(
        MqttClientOptions options,
        MqttClientFactory clientFactory,
        IServiceScopeFactory serviceScopeFactory,
        IDatabaseCache databaseCache,
        ILogger<RingContactStateIngestor> logger)
        : base("ring/+/alarm/+/contact/state", "Ha-RingContactStateIngest", options, clientFactory, serviceScopeFactory, logger)
    {
        _databaseCache = databaseCache;
    }

    protected override async Task OnMessageReceived(string payload, MqttApplicationMessageReceivedEventArgs e)
    {
        string[] parts = e.ApplicationMessage.Topic.Split('/');
        if (parts.Length < 6)
        {
            _logger.LogWarning("Invalid topic format for Ring Contact State: {Topic}", e.ApplicationMessage.Topic);
            return;
        }

        string serialNumber = parts[3];
        Sensor? sensor = await _databaseCache.GetSensorAsync(serialNumber);
        if (sensor is null)
        {
            _logger.LogWarning("Sensor with serial number {SerialNumber} not found in database.", serialNumber);
            return;
        }

        bool isClosed = string.Equals(payload, "off", StringComparison.OrdinalIgnoreCase);
        _logger.LogInformation("Received Ring Contact State for {SerialNumber}: Is Closed = {State}", serialNumber, isClosed);

        try
        {
            _logger.LogInformation("Inserting Ring Contact State into database for sensor {SerialNumber}: Is Closed = {State}", serialNumber, isClosed);
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            using HomeAutomationDb db = scope.ServiceProvider.GetRequiredService<HomeAutomationDb>();
            await db.InsertAsync(new SensorValue()
            {
                BoardSerialNumber = sensor.BoardSerialNumber,
                SensorSerialNumber = serialNumber,
                Timestamp = DateTimeOffset.UtcNow,
                Value = isClosed ? bool.TrueString : bool.FalseString,
            });
            _logger.LogInformation("Successfully inserted Ring Contact State into database for sensor {SerialNumber}.", serialNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to insert Ring Contact State into database for sensor {SerialNumber}.", serialNumber);
            return;
        }
    }
}