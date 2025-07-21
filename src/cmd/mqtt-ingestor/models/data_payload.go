package models

type DataPayload struct {
	CloudEvent
	DataValue any    `json:"data_value"`
	DataID    string `json:"data_id"`
}
