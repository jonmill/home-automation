using HomeAutomation.Database;
using HomeAutomation.Models.Database;
using HomeAutomation.Models.Enums;
using HomeAutomation.Models.Mqtt;
using HomeAutomation.PushExtensions;
using LinqToDB.Async;

namespace HomeAutomation.StateMachine.Models;

public sealed class HomeModel
{
    private const double INSIDE_TEMPERATURE_HIGH_THRESHOLD = 23.8; // 75 F in C
    private const double OUTSIDE_TEMPERATURE_HIGH_THRESHOLD = 28.3; // 83 F in C
    private const double INSIDE_TEMPERATURE_LOW_THRESHOLD = 15.5; // 60 F in C
    private const double OUTSIDE_TEMPERATURE_LOW_THRESHOLD = 10.0; // 50 F in C

    private static readonly string[] INSIDE_TEMP_SENSORS = ["9", "14", "20"];
    private static readonly string[] OUTSIDE_TEMP_SENSORS = ["17"];

    private readonly Dictionary<string, Sensor> _sensors;
    private readonly Dictionary<string, bool> _contactStates;
    private readonly Dictionary<string, double> _temperatureStates;
    private readonly Dictionary<string, bool> _temperatureOverThreshold;
    private readonly Dictionary<string, bool> _temperatureUnderThreshold;
    private readonly IPushNotifier _pushNotifier;
    private readonly ILogger<HomeModel> _logger;

    public HomeModel(IPushNotifier pushNotifier, ILogger<HomeModel> logger)
    {
        _pushNotifier = pushNotifier;
        _logger = logger;
        _sensors = [];
        _contactStates = [];
        _temperatureStates = [];
        _temperatureOverThreshold = [];
        _temperatureUnderThreshold = [];
    }

    public async Task InitializeAsync(HomeAutomationDb database)
    {
        await InitializeContactSensorsAsync(database);
        await InitializeTemperatureSensorsAsync(database);
    }

    private async Task InitializeContactSensorsAsync(HomeAutomationDb database)
    {
        List<Sensor> contactSensors = await database.Sensors.Where(s => s.Type == SensorTypes.Contact).ToListAsync();
        foreach (Sensor sensor in contactSensors)
        {
            _sensors[sensor.SerialNumber] = sensor;

            SensorValue? latestValue = await database.SensorValues
                .Where(sv => sv.SensorSerialNumber == sensor.SerialNumber)
                .OrderByDescending(sv => sv.Timestamp)
                .FirstOrDefaultAsync();
            if (latestValue is null)
            {
                _logger.LogWarning("No sensor values found for sensor ID {SensorId}", sensor.SerialNumber);
                _contactStates[sensor.SerialNumber] = false; // Default to false if no data
            }
            else if (bool.TryParse(latestValue.Value, out bool parsedValue))
            {
                _contactStates[sensor.SerialNumber] = parsedValue;
            }
            else
            {
                _logger.LogWarning("Unable to parse sensor value '{SensorValue}' for sensor ID {SensorId}", latestValue.Value, sensor.SerialNumber);
                _contactStates[sensor.SerialNumber] = false; // Default to false if parsing fails
            }
        }
    }

    private async Task InitializeTemperatureSensorsAsync(HomeAutomationDb database)
    {
        List<Sensor> temperatureSensors = await database.Sensors.Where(s => s.Type == SensorTypes.Temperature).ToListAsync();
        foreach (Sensor sensor in temperatureSensors)
        {
            _sensors[sensor.SerialNumber] = sensor;

            SensorValue? latestValue = await database.SensorValues
                .Where(sv => sv.SensorSerialNumber == sensor.SerialNumber)
                .OrderByDescending(sv => sv.Timestamp)
                .FirstOrDefaultAsync();
            if (latestValue is null)
            {
                _logger.LogWarning("No sensor values found for sensor ID {SensorId}", sensor.SerialNumber);
                _temperatureStates[sensor.SerialNumber] = 0.0; // Default to 0.0 if no data
                _temperatureOverThreshold[sensor.SerialNumber] = false;
                _temperatureUnderThreshold[sensor.SerialNumber] = false;
            }
            else if (double.TryParse(latestValue.Value, out double parsedValue))
            {
                double highThreshold = INSIDE_TEMP_SENSORS.Contains(sensor.SerialNumber) ? INSIDE_TEMPERATURE_HIGH_THRESHOLD : OUTSIDE_TEMPERATURE_HIGH_THRESHOLD;
                double lowThreshold = INSIDE_TEMP_SENSORS.Contains(sensor.SerialNumber) ? INSIDE_TEMPERATURE_LOW_THRESHOLD : OUTSIDE_TEMPERATURE_LOW_THRESHOLD;
                _temperatureStates[sensor.SerialNumber] = Math.Round(parsedValue, 1);
                _temperatureOverThreshold[sensor.SerialNumber] = parsedValue > highThreshold;
                _temperatureUnderThreshold[sensor.SerialNumber] = parsedValue < lowThreshold;
            }
            else
            {
                _logger.LogWarning("Unable to parse sensor value '{SensorValue}' for sensor ID {SensorId}", latestValue.Value, sensor.SerialNumber);
                _temperatureStates[sensor.SerialNumber] = 0.0; // Default to 0.0 if parsing fails
                _temperatureOverThreshold[sensor.SerialNumber] = false;
                _temperatureUnderThreshold[sensor.SerialNumber] = false;
            }
        }
    }

    private async Task InternalUpdateContactSensorAsync(Sensor sensor, bool newState)
    {
        _logger.LogInformation("Sensor {SensorId} state received: {SensorState}", sensor.SerialNumber, newState);

        bool sensorStateChanged = false;
        lock (_sensors[sensor.SerialNumber])
        {
            bool currentState = _contactStates[sensor.SerialNumber];
            if (currentState != newState)
            {
                _contactStates[sensor.SerialNumber] = newState;
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
            await _pushNotifier.NotifyAsync(title, message, highPriority: false);
            _logger.LogInformation("Push Notification sent: '{Title}'", title);
        }
    }

    public Task UpdateContactSensorStateAsync(Sensor sensor, bool newState)
    {
        if (sensor is null)
        {
            return Task.CompletedTask;
        }
        else if (_sensors.ContainsKey(sensor.SerialNumber) == false)
        {
            return Task.CompletedTask;
        }

        return InternalUpdateContactSensorAsync(sensor, newState);
    }

    public Task UpdateContactSensorStateAsync(Sensor sensor, DataPayload sensorValue)
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

        return InternalUpdateContactSensorAsync(sensor, parsedValue);
    }

    public async Task UpdateTemperatureSensorStateAsync(Sensor sensor, double newState)
    {
        if (sensor is null)
        {
            return;
        }
        else if (_sensors.ContainsKey(sensor.SerialNumber) == false)
        {
            return;
        }

        // Check if the current state has changed
        newState = Math.Round(newState, 1);
        lock (_sensors[sensor.SerialNumber])
        {
            double currentState = _temperatureStates[sensor.SerialNumber];
            if (Math.Abs(currentState - newState) > 0.1)
            {
                _temperatureStates[sensor.SerialNumber] = newState;
                _logger.LogInformation("Sensor {SensorId} temperature state changed from {OldState} to {NewState}", sensor.SerialNumber, currentState, newState);
            }
            else
            {
                return;
            }
        }

        // Check our tri-state for movement into/out of bad states
        short temperatureState;
        double highThreshold = INSIDE_TEMP_SENSORS.Contains(sensor.SerialNumber) ? INSIDE_TEMPERATURE_HIGH_THRESHOLD : OUTSIDE_TEMPERATURE_HIGH_THRESHOLD;
        double lowThreshold = INSIDE_TEMP_SENSORS.Contains(sensor.SerialNumber) ? INSIDE_TEMPERATURE_LOW_THRESHOLD : OUTSIDE_TEMPERATURE_LOW_THRESHOLD;
        if (newState > highThreshold)
        {
            temperatureState = 1;
        }
        else if (newState < lowThreshold)
        {
            temperatureState = -1;
        }
        else
        {
            temperatureState = 0;
        }

        // Check if we moved out of a bad state into a good one
        string message;
        double cToF = Math.Round(highThreshold * 9.0 / 5.0 + 32.0, 0);
        bool urgent = false;
        if ((temperatureState == 0) && _temperatureOverThreshold[sensor.SerialNumber])
        {
            // We moved from high-temp to good temp
            message = $"{sensor.Name} has dropped to {cToF}°F";
        }
        else if ((temperatureState == 0) && _temperatureUnderThreshold[sensor.SerialNumber])
        {
            // We moved from low-temp to good temp
            message = $"{sensor.Name} has risen to {cToF}°F";
        }
        else if ((temperatureState == 1) && _temperatureOverThreshold[sensor.SerialNumber])
        {
            // No-op - we're still over the high threshold
            return;
        }
        else if ((temperatureState == 1) && _temperatureUnderThreshold[sensor.SerialNumber])
        {
            // We moved from low-temp to high-temp. This is weird
            message = $"{sensor.Name} has risen to {cToF}°F";
            urgent = true;
        }
        else if ((temperatureState == -1) && _temperatureUnderThreshold[sensor.SerialNumber])
        {
            // No-op - we're still under the low threshold
            return;
        }
        else if ((temperatureState == -1) && _temperatureOverThreshold[sensor.SerialNumber])
        {
            // We moved from high-temp to low-temp. This is weird
            message = $"{sensor.Name} has dropped to {cToF}°F";
            urgent = true;
        }
        else
        {
            _logger.LogWarning(
                "Unhandled temperature state transition for sensor {SensorId} with new state {NewState}:{OverThreshold}:{UnderThreshold}:{TemperatureMovement}",
                sensor.SerialNumber,
                newState,
                _temperatureOverThreshold[sensor.SerialNumber],
                _temperatureUnderThreshold[sensor.SerialNumber],
                temperatureState);
            return;
        }

        string title = $"{sensor.Name} Alert";
        await _pushNotifier.NotifyAsync(title, message, urgent);
        _logger.LogInformation("Push Notification sent: '{Title}'", title);
    }
}