package handlers

import (
	"encoding/json"

	"home-automation/src/cmd/mqtt-ingestor/models"

	mqtt "github.com/eclipse/paho.mqtt.golang"
	"go.uber.org/zap"
)

func (h *HandlerContext) HandleAirPressureMessage(_ mqtt.Client, msg mqtt.Message) {
	var payload models.DataPayload
	if err := json.Unmarshal(msg.Payload(), &payload); err != nil {
		h.Logger.Warn("Failed to unmarshal Air Pressure message", zap.Error(err))
		return
	}

	h.Logger.Info("Air Pressure State Change", zap.Float64("new_pressure_state", payload.DataValue.(float64)), zap.Int("board_id", payload.Source), zap.String("sensor_id", payload.DataID))
}
