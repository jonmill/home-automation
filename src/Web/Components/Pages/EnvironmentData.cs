using HomeAutomation.Models.Database;
using HomeAutomation.Web.Components.Helpers;
using MudBlazor;

namespace HomeAutomation.Web.Components.Pages;

public record EnvironmentData
{
    private double _temperature;
    private DateTimeOffset _tempTimestamp;
    private double _humidity;
    private DateTimeOffset _humidityTimestamp;
    private double _pressure;
    private DateTimeOffset _pressureTimestamp;
    private double _aqi;
    private DateTimeOffset _aqiTimestamp;

    public EnvironmentData()
    {
        _temperature = double.MaxValue;
        _tempTimestamp = DateTimeOffset.MinValue;
        _humidity = double.MaxValue;
        _humidityTimestamp = DateTimeOffset.MinValue;
        _pressure = double.MaxValue;
        _pressureTimestamp = DateTimeOffset.MinValue;
        _aqi = double.MaxValue;
        _aqiTimestamp = DateTimeOffset.MinValue;
    }

    public EnvironmentData(SensorValue? valTemp, SensorValue? valHumidity, SensorValue? valPressure, SensorValue? valAQI)
    {
        if (double.TryParse(valTemp?.Value, out double temp))
        {
            _temperature = temp;
            _tempTimestamp = valTemp.Timestamp;
        }
        else
        {
            _temperature = 0.0;
            _tempTimestamp = DateTimeOffset.MinValue;
        }

        if (double.TryParse(valHumidity?.Value, out double humidity))
        {
            _humidity = humidity;
            _humidityTimestamp = valHumidity.Timestamp;
        }
        else
        {
            _humidity = 0.0;
            _humidityTimestamp = DateTimeOffset.MinValue;
        }

        if (double.TryParse(valPressure?.Value, out double pressure))
        {
            _pressure = pressure;
            _pressureTimestamp = valPressure.Timestamp;
        }
        else
        {
            _pressure = 0.0;
            _pressureTimestamp = DateTimeOffset.MinValue;
        }

        if (double.TryParse(valAQI?.Value, out double aqi))
        {
            _aqi = aqi;
            _aqiTimestamp = valAQI.Timestamp;
        }
        else
        {
            _aqi = 0.0;
            _aqiTimestamp = DateTimeOffset.MinValue;
        }
    }

    private double RoundedTemperature => Math.Round((_temperature * 9.0 / 5.0) + 32, 0);
    public string TemperatureString => $"{RoundedTemperature} °F";
    public string TemperatureLastMeasurement => DateTimeHelpers.GetFriendlyTimeString(_tempTimestamp);
    public string TemperatureStyle => RoundedTemperature switch
    {
        <= 32 => Colors.Blue.Darken2,
        <= 55 => Colors.Blue.Lighten2,
        <= 68 => Colors.Orange.Lighten3,
        <= 73 => Colors.Green.Darken1,
        <= 80 => Colors.Orange.Lighten3,
        _ => Colors.Red.Darken1
    };

    private double RoundedHumidity => Math.Round(_humidity, 0);
    public string HumidityString => $"{RoundedHumidity}%";
    public string HumidityLastMeasurement => DateTimeHelpers.GetFriendlyTimeString(_humidityTimestamp);
    public string HumidityStyle => RoundedHumidity switch
    {
        <= 30 => Colors.Red.Darken1,
        <= 50 => Colors.Orange.Darken1,
        <= 65 => Colors.Green.Darken1,
        <= 75 => Colors.Orange.Darken1,
        <= 90 => Colors.Red.Darken1,
        _ => Colors.Red.Darken4
    };

    private double RoundedPressure => Math.Round(_pressure, 0);
    public string PressureString => $"{RoundedPressure} hPa";
    public string PressureLastMeasurement => DateTimeHelpers.GetFriendlyTimeString(_pressureTimestamp);
    public string PressureStyle => RoundedPressure switch
    {
        <= 980 => Colors.Blue.Darken2,
        <= 1009 => Colors.Blue.Lighten2,
        <= 1023 => Colors.Green.Darken1,
        <= 1040 => Colors.Orange.Darken1,
        _ => Colors.Red.Darken1
    };

    private double RoundedAQI => Math.Round(_aqi, 0);
    public string AQIString => $"{RoundedAQI}";
    public string AQILastMeasurement => DateTimeHelpers.GetFriendlyTimeString(_aqiTimestamp);
    public string AQIStyle => RoundedAQI switch
    {
        <= 50 => Colors.Green.Darken1,
        <= 100 => Colors.Orange.Darken1,
        <= 150 => Colors.Orange.Darken2,
        <= 200 => Colors.Red.Darken1,
        <= 300 => Colors.Red.Darken2,
        _ => Colors.Red.Darken4
    };
}