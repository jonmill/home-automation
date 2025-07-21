package handlers

import (
	"encoding/json"
	"strings"

	"home-automation/src/cmd/mqtt-ingestor/models"

	mqtt "github.com/eclipse/paho.mqtt.golang"
	"go.uber.org/zap"
)

func (h *HandlerContext) HandleLogMessage(_ mqtt.Client, msg mqtt.Message) {

	clean := strings.ReplaceAll(string(msg.Payload()), "\x1b", "")

	var payload models.LogEvent
	if err := json.Unmarshal([]byte(clean), &payload); err != nil {
		h.Logger.Warn("Failed to unmarshal log message", zap.Error(err))
		return
	}

	h.Logger.Info("Board Log Entry", zap.String("message", payload.Message), zap.String("tag", payload.Tag), zap.Int("board_id", payload.Source))
}
