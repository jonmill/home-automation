package dbmodels

import (
	"time"
)

// SensorType represents the type of sensor.
type SensorType string

const (
	SensorTypeTemperature SensorType = "temperature"
	SensorTypeHumidity    SensorType = "humidity"
	SensorTypeLux         SensorType = "lux"
	SensorTypeMotion      SensorType = "motion"
	SensorTypePressure    SensorType = "pressure"
	SensorTypeVoltage     SensorType = "voltage"
)

// UnitOfMeasure represents the unit associated with the sensor.
type UnitOfMeasure string

const (
	UnitCelsius    UnitOfMeasure = "C"
	UnitFahrenheit UnitOfMeasure = "F"
	UnitPercent    UnitOfMeasure = "%"
	UnitLux        UnitOfMeasure = "lux"
	UnitPascal     UnitOfMeasure = "Pa"
	UnitVolts      UnitOfMeasure = "V"
)

// Board represents an ESPHome device board.
type Board struct {
	ID        int       `db:"id"`
	Name      string    `db:"name"`
	AddedAt   time.Time `db:"added_at"`
	OnBattery bool      `db:"on_battery"`
}

// Sensor represents a physical sensor.
type Sensor struct {
	ID            int           `db:"id"`
	Name          string        `db:"name"`
	AddedAt       time.Time     `db:"added_at"`
	Type          SensorType    `db:"type"`
	UnitOfMeasure UnitOfMeasure `db:"unit"`
}

// BoardSensor maps sensors to boards.
type BoardSensor struct {
	BoardID  int `db:"board_id"`
	SensorID int `db:"sensor_id"`
}

// SensorValue represents a measurement from a sensor on a board.
type SensorValue struct {
	BoardID    int       `db:"board_id"`
	SensorID   int       `db:"sensor_id"`
	RecordedAt time.Time `db:"recorded_at"`
	Value      string    `db:"value"`
}

// LogEntry stores logs from a board.
type LogEntry struct {
	ID       int64     `db:"id"`
	BoardID  int       `db:"board_id"`
	Message  string    `db:"message"`
	Level    int       `db:"level"`
	Tag      string    `db:"tag"`
	LoggedAt time.Time `db:"logged_at"`
}
