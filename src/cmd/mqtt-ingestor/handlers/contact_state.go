package handlers

import (
	"encoding/json"

	mqttmodels "home-automation/src/cmd/mqtt-ingestor/internal/mqtt/models"

	mqtt "github.com/eclipse/paho.mqtt.golang"
	"go.uber.org/zap"
)

func (h *HandlerContext) HandleContactStateMessage(_ mqtt.Client, msg mqtt.Message) {
	var payload mqttmodels.DataPayload
	if err := json.Unmarshal(msg.Payload(), &payload); err != nil {
		h.Logger.Warn("Failed to unmarshal Contact State message", zap.Error(err))
		return
	}

	h.Logger.Info("Contact State Change", zap.Bool("new_contact_state", payload.DataValue.(bool)), zap.Int("board_id", payload.Source), zap.String("sensor_id", payload.DataID))
}
