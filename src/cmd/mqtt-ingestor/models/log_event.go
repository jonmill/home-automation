package models

type LogEvent struct {
	CloudEvent
	Level   int    `json:"data_level"`
	Message string `json:"data_message"`
	Tag     string `json:"data_tag"`
}
