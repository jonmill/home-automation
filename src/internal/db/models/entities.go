package dbmodels

import (
	"time"
)

// SensorType represents the type of sensor.
type SensorType int

const (
	SensorTypeTemperature SensorType = iota
	SensorTypeHumidity
	SensorTypeLux
	SensorTypeMotion
	SensorTypePressure
	SensorTypeVoltage
	SensorTypeContact
	SensorTypeOrganicCompound
)

// UnitOfMeasure represents the unit associated with the sensor.
type UnitOfMeasure int

const (
	UnitCelsius UnitOfMeasure = iota
	UnitPercent
	UnitLux
	UnitPascal
	UnitVolts
	UnitBoolean
	UnitPartPerBillion
)

// Board represents an ESPHome device board.
type Board struct {
	ID        int       `db:"id"`
	Name      string    `db:"name"`
	AddedAt   time.Time `db:"added_at"`
	OnBattery bool      `db:"on_battery"`
	IsDeleted bool      `db:"is_deleted"`
	DeletedOn time.Time `db:"deleted_on"`
}

// Sensor represents a physical sensor.
type Sensor struct {
	ID            int           `db:"id"`
	Name          string        `db:"name"`
	AddedAt       time.Time     `db:"added_at"`
	Type          SensorType    `db:"type"`
	UnitOfMeasure UnitOfMeasure `db:"unit"`
	IsDeleted     bool          `db:"is_deleted"`
	DeletedOn     time.Time     `db:"deleted_on"`
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
