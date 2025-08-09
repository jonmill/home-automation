package handlers

import (
	"encoding/json"

	mqttmodels "home-automation/internal/mqtt/models"

	mqtt "github.com/eclipse/paho.mqtt.golang"
	"go.uber.org/zap"
)

func (h *HandlerContext) HandleBoardTemperatureMessage(_ mqtt.Client, msg mqtt.Message) {
	var payload mqttmodels.DataPayload
	if err := json.Unmarshal(msg.Payload(), &payload); err != nil {
		h.Logger.Warn("Failed to unmarshal Board Temperature message", zap.Error(err))
		return
	}

	h.Logger.Info("Board Temperature State Change", zap.Float64("new_temperature_state", payload.DataValue.(float64)), zap.Int("board_id", payload.Source))
	/*if err := h.DbCache.SaveSensorData(h.AppCtx, payload); err != nil {
		h.Logger.Error("Failed to insert sensor value into DB", zap.Error(err), zap.Int("board_id", payload.Source), zap.String("sensor_id", payload.DataID))
		return
	}*/
}

func (h *HandlerContext) HandleAirHumidityMessage(_ mqtt.Client, msg mqtt.Message) {
	var payload mqttmodels.DataPayload
	if err := json.Unmarshal(msg.Payload(), &payload); err != nil {
		h.Logger.Warn("Failed to unmarshal Air Humidity message", zap.Error(err))
		return
	}

	h.Logger.Info("Air Humidity State Change", zap.Float64("new_humidity_state", payload.DataValue.(float64)), zap.Int("board_id", payload.Source), zap.String("sensor_id", payload.DataID))
	if err := h.DbCache.SaveSensorData(h.AppCtx, payload); err != nil {
		h.Logger.Error("Failed to save Air Humidity state", zap.Error(err))
	}
}

func (h *HandlerContext) HandleAirPressureMessage(_ mqtt.Client, msg mqtt.Message) {
	var payload mqttmodels.DataPayload
	if err := json.Unmarshal(msg.Payload(), &payload); err != nil {
		h.Logger.Warn("Failed to unmarshal Air Pressure message", zap.Error(err))
		return
	}

	h.Logger.Info("Air Pressure State Change", zap.Float64("new_pressure_state", payload.DataValue.(float64)), zap.Int("board_id", payload.Source), zap.String("sensor_id", payload.DataID))
	if err := h.DbCache.SaveSensorData(h.AppCtx, payload); err != nil {
		h.Logger.Error("Failed to save Air Pressure state", zap.Error(err))
	}
}

func (h *HandlerContext) HandleAirTemperatureMessage(_ mqtt.Client, msg mqtt.Message) {
	var payload mqttmodels.DataPayload
	if err := json.Unmarshal(msg.Payload(), &payload); err != nil {
		h.Logger.Warn("Failed to unmarshal Air Temperature message", zap.Error(err))
		return
	}

	h.Logger.Info("Air Temperature State Change", zap.Float64("new_temperature_state", payload.DataValue.(float64)), zap.Int("board_id", payload.Source), zap.String("sensor_id", payload.DataID))
	if err := h.DbCache.SaveSensorData(h.AppCtx, payload); err != nil {
		h.Logger.Error("Failed to save Air Temperature state", zap.Error(err))
	}
}

func (h *HandlerContext) HandleAirQualityMessage(_ mqtt.Client, msg mqtt.Message) {
	var payload mqttmodels.DataPayload
	if err := json.Unmarshal(msg.Payload(), &payload); err != nil {
		h.Logger.Warn("Failed to unmarshal Air Quality message", zap.Error(err))
		return
	}

	h.Logger.Info("Air Quality State Change", zap.Float64("new_air_quality_state", payload.DataValue.(float64)), zap.Int("board_id", payload.Source), zap.String("sensor_id", payload.DataID))
	if err := h.DbCache.SaveSensorData(h.AppCtx, payload); err != nil {
		h.Logger.Error("Failed to save Air Quality state", zap.Error(err))
	}
}

func (h *HandlerContext) HandleContactStateMessage(_ mqtt.Client, msg mqtt.Message) {
	var payload mqttmodels.DataPayload
	if err := json.Unmarshal(msg.Payload(), &payload); err != nil {
		h.Logger.Warn("Failed to unmarshal Contact State message", zap.Error(err))
		return
	}

	h.Logger.Info("Contact State Change", zap.Bool("new_contact_state", payload.DataValue.(bool)), zap.Int("board_id", payload.Source), zap.String("sensor_id", payload.DataID))
	if err := h.DbCache.SaveSensorData(h.AppCtx, payload); err != nil {
		h.Logger.Error("Failed to save Contact State", zap.Error(err))
	}
}

func (h *HandlerContext) HandleLuxMessage(_ mqtt.Client, msg mqtt.Message) {
	var payload mqttmodels.DataPayload
	if err := json.Unmarshal(msg.Payload(), &payload); err != nil {
		h.Logger.Warn("Failed to unmarshal Lux message", zap.Error(err))
		return
	}

	h.Logger.Info("Board Lux State Change", zap.Float64("new_lux_state", payload.DataValue.(float64)), zap.Int("board_id", payload.Source), zap.String("sensor_id", payload.DataID))
	if err := h.DbCache.SaveSensorData(h.AppCtx, payload); err != nil {
		h.Logger.Error("Failed to save Lux state", zap.Error(err))
	}
}
