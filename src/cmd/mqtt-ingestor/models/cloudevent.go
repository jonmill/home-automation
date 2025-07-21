package models

type CloudEvent struct {
	Source      int    `json:"source"`
	Time        string `json:"time"`
	ContentType string `json:"content_type"`
}
