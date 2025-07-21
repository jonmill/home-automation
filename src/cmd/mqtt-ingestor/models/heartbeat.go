package models

type Heartbeat struct {
	CloudEvent
	NextHeartbeatInSeconds int `json:"data_next_heartbeat_in_seconds"`
}
