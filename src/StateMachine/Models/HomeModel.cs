using HomeAutomation.Database;
using HomeAutomation.Models.Database;
using HomeAutomation.Models.Mqtt;
using HomeAutomation.PushExtensions;
using LinqToDB.Async;

namespace HomeAutomation.StateMachine.Models;

public sealed class HomeModel
{
    private readonly Dictionary<string, Sensor> _sensors;
    private readonly Dictionary<string, bool> _sensorStates;
    private readonly IPushNotifier _pushNotifier;
    private readonly ILogger<HomeModel> _logger;

    public HomeModel(IPushNotifier pushNotifier, ILogger<HomeModel> logger)
    {
        _pushNotifier = pushNotifier;
        _logger = logger;
        _sensors = [];
        _sensorStates = [];
    }

    public async Task InitializeAsync(HomeAutomationDb database)
    {
        List<string> ids = ["1", "2", "3", "4", "5", "6", "7", "8", "ae698f45-bb4b-478d-9a6b-438f334b596b", "d13f7da9-ead4-4d8d-9812-ae57b98e5491", "cd4ed731-1f88-4f2a-9ae8-5c4fe90fd71d", "a3ba703d-2e22-4b32-ae15-ef3d12ea9014", "1e4e05b8-eb43-4877-8a8c-7ed87cbf74b7", "d54ccc81-3601-4b7b-b2ca-191687f70237", "b50ca877-31fa-4fe6-9026-e250e17558e5", "01c72dde-2213-48dc-b548-c2b0ed215dff", "2b07e27c-9481-4915-b6c0-5bee2aab6f3d"];
        foreach (string id in ids)
        {
            Sensor? sensor = await database.Sensors.SingleOrDefaultAsync(s => s.SerialNumber == id);
            if (sensor is null)
            {
                _logger.LogError("Sensor with ID {SensorId} not found in database", id);
                continue;
            }
            else
            {
                _sensors[id] = sensor;
            }

            SensorValue? latestValue = await database.SensorValues
                .Where(sv => sv.SensorSerialNumber == id)
                .OrderByDescending(sv => sv.Timestamp)
                .FirstOrDefaultAsync();
            if (latestValue is null)
            {
                _logger.LogWarning("No sensor values found for sensor ID {SensorId}", id);
                _sensorStates[id] = false; // Default to false if no data
            }
            else if (bool.TryParse(latestValue.Value, out bool parsedValue))
            {
                _sensorStates[id] = parsedValue;
            }
            else
            {
                _logger.LogWarning("Unable to parse sensor value '{SensorValue}' for sensor ID {SensorId}", latestValue.Value, id);
                _sensorStates[id] = false; // Default to false if parsing fails
            }
        }
    }

    private async Task InternalUpdateSensorAsync(Sensor sensor, bool newState)
    {
        _logger.LogInformation("Sensor {SensorId} state received: {SensorState}", sensor.SerialNumber, newState);

        bool sensorStateChanged = false;
        lock (_sensors[sensor.SerialNumber])
        {
            bool currentState = _sensorStates[sensor.SerialNumber];
            if (currentState != newState)
            {
                _sensorStates[sensor.SerialNumber] = newState;
                sensorStateChanged = true;
                _logger.LogInformation("Sensor {SensorId} state changed from {OldState} to {NewState}", sensor.SerialNumber, currentState, newState);
            }
        }

        if (sensorStateChanged)
        {
            // We'll cheat with the check for door / window (for the title) by checking serial numbers.
            // Ring assigns long strings for serial numbers whereas ours are incrementing ints
            string entityType = sensor.SerialNumber.Length > 3 ? "Window" : "Door";
            string sensorName = sensor.Name;
            string state = newState ? "Open" : "Closed";
            string title = $"{entityType} {state}";
            string message = $"{sensorName} is now {state}";
            await _pushNotifier.NotifyAsync(title, message);
            _logger.LogInformation("Push Notification sent: '{Title}'", title);
        }
    }

    public Task UpdateSensorStateAsync(Sensor sensor, bool newState)
    {
        if (sensor is null)
        {
            return Task.CompletedTask;
        }
        else if (_sensors.ContainsKey(sensor.SerialNumber) == false)
        {
            return Task.CompletedTask;
        }

        return InternalUpdateSensorAsync(sensor, newState);
    }

    public Task UpdateSensorStateAsync(Sensor sensor, DataPayload sensorValue)
    {
        bool parsedValue = false;
        if (_sensors.ContainsKey(sensor.SerialNumber) == false)
        {
            return Task.CompletedTask;
        }
        else if (bool.TryParse(sensorValue.SensorValue, out parsedValue) == false)
        {
            _logger.LogWarning("Unable to parse sensor value '{SensorValue}' for sensor ID {SensorId}", sensorValue.SensorValue, sensor.SerialNumber);
            return Task.CompletedTask;
        }

        return InternalUpdateSensorAsync(sensor, parsedValue);
    }
}