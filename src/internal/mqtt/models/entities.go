package mqttmodels

// CloudEvent represents metadata associated with an MQTT message.
// Source identifies the board sending the event. Time and ContentType
// mirror CloudEvents fields.
type CloudEvent struct {
	Source      int    `json:"source"`
	Time        string `json:"time"`
	ContentType string `json:"content_type"`
}

// DataPayload wraps a sensor reading or other data value.
type DataPayload struct {
	CloudEvent
	DataValue any    `json:"data_value"`
	DataID    string `json:"data_id"`
}

// Heartbeat represents a periodic heartbeat message from a board.
type Heartbeat struct {
	CloudEvent
	NextHeartbeatInSeconds int `json:"data_next_heartbeat_in_seconds"`
}

// LogEvent contains a log entry emitted by a board.
type LogEvent struct {
	CloudEvent
	Level   int    `json:"data_level"`
	Message string `json:"data_message"`
	Tag     string `json:"data_tag"`
}
