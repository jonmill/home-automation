package handlers

import (
	"encoding/json"

	"home-automation/src/cmd/mqtt-ingestor/models"

	mqtt "github.com/eclipse/paho.mqtt.golang"
	"go.uber.org/zap"
)

func (h *HandlerContext) HandlePowerStateMessage(_ mqtt.Client, msg mqtt.Message) {
	var payload models.DataPayload
	if err := json.Unmarshal(msg.Payload(), &payload); err != nil {
		h.Logger.Warn("Failed to unmarshal Power State message", zap.Error(err))
		return
	}

	h.Logger.Info("Board Power State Change", zap.String("new_power_state", payload.DataValue.(string)), zap.Int("board_id", payload.Source))
}
