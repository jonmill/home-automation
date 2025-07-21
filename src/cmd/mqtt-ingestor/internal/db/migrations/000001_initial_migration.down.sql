DROP INDEX IF EXISTS idx_sensor_values_recorded_at;
DROP INDEX IF EXISTS idx_log_entries_logged_at;

DROP TABLE IF EXISTS LogEntries;
DROP TABLE IF EXISTS SensorValues;
DROP TABLE IF EXISTS BoardSensors;
DROP TABLE IF EXISTS Sensors;
DROP TABLE IF EXISTS Boards;