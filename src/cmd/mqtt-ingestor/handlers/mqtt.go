package handlers

import (
	"encoding/json"

	mqttmodels "home-automation/src/cmd/mqtt-ingestor/internal/mqtt/models"

	mqtt "github.com/eclipse/paho.mqtt.golang"
	"go.uber.org/zap"
)

func (h *HandlerContext) HandleMqttMessage(_ mqtt.Client, msg mqtt.Message) {
	var payload mqttmodels.DataPayload
	if err := json.Unmarshal(msg.Payload(), &payload); err != nil {
		h.Logger.Warn("Failed to unmarshal MQTT message", zap.Error(err))
		return
	}

	h.Logger.Info("Board MQTT State Change", zap.String("new_mqtt_state", payload.DataValue.(string)), zap.Int("board_id", payload.Source))
}
