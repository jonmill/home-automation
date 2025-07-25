package main

import (
	"context"
	"fmt"
	"os"
	"os/signal"
	"syscall"

	"home-automation/src/cmd/mqtt-ingestor/handlers"
	"home-automation/src/internal/db"

	mqtt "github.com/eclipse/paho.mqtt.golang"
	"github.com/jackc/pgx/v5/pgxpool"
	"go.uber.org/zap"
)

var dbContext *pgxpool.Pool
var logger *zap.Logger

func main() {
	ctx, cancel := context.WithCancel(context.Background())
	defer cancel()

	var err error
	logger, err = zap.NewDevelopment()
	if err != nil {
		panic(fmt.Sprintf("failed to init zap logger: %v", err))
	}
	defer logger.Sync()

	// Connect to Postgres
	dbURL := os.Getenv("DATABASE_URL")
	if dbURL == "" {
		dbURL = "postgres://homeauto:homeauto@localhost:5432/homeautomation"
	}
	logger.Info("Running database migrations...")
	if err := db.RunMigrations(dbURL); err != nil {
		logger.Fatal("Database migration failed", zap.Error(err))
	}
	dbContext, err = pgxpool.New(ctx, dbURL)
	if err != nil {
		logger.Fatal("Failed to connect to DB", zap.Error(err))
	}
	defer dbContext.Close()

	// Connect to MQTT
	mqttUrl := os.Getenv("MQTT_URL")
	mqttPassword := os.Getenv("MQTT_PASSWORD")
	if mqttUrl == "" {
		mqttUrl = "tcp://localhost:1883"
	}
	mqttOpts := mqtt.NewClientOptions()
	mqttOpts.AddBroker(mqttUrl)
	mqttOpts.SetClientID("mqtt-ingestor")
	mqttOpts.SetUsername("ha-mqtt")
	mqttOpts.SetPassword(mqttPassword)
	client := mqtt.NewClient(mqttOpts)

	hctx := &handlers.HandlerContext{
		Logger: logger,
		AppCtx: ctx,
		DB:     dbContext,
	}

	hctx.DbCache, err = db.NewSensorCacheContext(ctx, dbContext, logger)
	if err != nil {
		logger.Fatal("Failed to create sensor cache context", zap.Error(err))
	}

	if token := client.Connect(); token.Wait() && token.Error() != nil {
		logger.Fatal("MQTT connect error", zap.Error(token.Error()))
	}
	defer client.Disconnect(250)

	// Subscribe to our topics
	client.Subscribe("board/+/logging", 1, hctx.HandleLogMessage)
	client.Subscribe("board/+/mqtt", 1, hctx.HandleMqttMessage)
	client.Subscribe("board/+/data", 1, hctx.HandleOtaMessage)
	client.Subscribe("board/+/power", 1, hctx.HandlePowerStateMessage)
	client.Subscribe("board/+/internal_temperature", 1, hctx.HandleBoardTemperatureMessage)
	client.Subscribe("board/+/sensor/+/lux", 1, hctx.HandleLuxMessage)
	client.Subscribe("board/+/sensor/+/air_temperature", 1, hctx.HandleAirTemperatureMessage)
	client.Subscribe("board/+/sensor/+/air_humidity", 1, hctx.HandleAirHumidityMessage)
	client.Subscribe("board/+/sensor/+/air_pressure", 1, hctx.HandleAirPressureMessage)
	client.Subscribe("board/+/sensor/+/contact_state", 1, hctx.HandleContactStateMessage)

	// Wait for shutdown
	sig := make(chan os.Signal, 1)
	signal.Notify(sig, os.Interrupt, syscall.SIGTERM)
	<-sig
	logger.Info("Shutting down...")
}
