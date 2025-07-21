package mqttmodels

type CloudEvent struct {
	Source      int    `json:"source"`
	Time        string `json:"time"`
	ContentType string `json:"content_type"`
}

type DataPayload struct {
	CloudEvent
	DataValue any    `json:"data_value"`
	DataID    string `json:"data_id"`
}

type Heartbeat struct {
	CloudEvent
	NextHeartbeatInSeconds int `json:"data_next_heartbeat_in_seconds"`
}

type LogEvent struct {
	CloudEvent
	Level   int    `json:"data_level"`
	Message string `json:"data_message"`
	Tag     string `json:"data_tag"`
}
