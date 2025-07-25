package handlers

import (
	"encoding/json"
	"strings"

	mqttmodels "home-automation/src/cmd/mqtt-ingestor/internal/mqtt/models"

	mqtt "github.com/eclipse/paho.mqtt.golang"
	"go.uber.org/zap"
)

func (h *HandlerContext) HandleLogMessage(_ mqtt.Client, msg mqtt.Message) {

	clean := strings.ReplaceAll(string(msg.Payload()), "\x1b", "")

	var payload mqttmodels.LogEvent
	if err := json.Unmarshal([]byte(clean), &payload); err != nil {
		h.Logger.Warn("Failed to unmarshal log message", zap.Error(err))
		return
	}

	h.Logger.Info("Board Log Entry", zap.String("message", payload.Message), zap.String("tag", payload.Tag), zap.Int("board_id", payload.Source))
}

func (h *HandlerContext) HandleMqttMessage(_ mqtt.Client, msg mqtt.Message) {
	var payload mqttmodels.DataPayload
	if err := json.Unmarshal(msg.Payload(), &payload); err != nil {
		h.Logger.Warn("Failed to unmarshal MQTT message", zap.Error(err))
		return
	}

	h.Logger.Info("Board MQTT State Change", zap.String("new_mqtt_state", payload.DataValue.(string)), zap.Int("board_id", payload.Source))
}

func (h *HandlerContext) HandleOtaMessage(_ mqtt.Client, msg mqtt.Message) {
	var payload mqttmodels.DataPayload
	if err := json.Unmarshal(msg.Payload(), &payload); err != nil {
		h.Logger.Warn("Failed to unmarshal OTA message", zap.Error(err))
		return
	}

	h.Logger.Info("Board OTA State Change", zap.Float64("new_ota_state", payload.DataValue.(float64)), zap.Int("board_id", payload.Source))
}

func (h *HandlerContext) HandlePowerStateMessage(_ mqtt.Client, msg mqtt.Message) {
	var payload mqttmodels.DataPayload
	if err := json.Unmarshal(msg.Payload(), &payload); err != nil {
		h.Logger.Warn("Failed to unmarshal Power State message", zap.Error(err))
		return
	}

	h.Logger.Info("Board Power State Change", zap.String("new_power_state", payload.DataValue.(string)), zap.Int("board_id", payload.Source))
}
