namespace HomeAutomation.Models.Mqtt;

public enum NewDataEventTypes : byte
{
    Heartbeat = 0,
    LogEvent = 1,
    SensorData = 2,
    RingMetadata = 3
}