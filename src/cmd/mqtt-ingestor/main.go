package main

import (
	"context"
	"fmt"
	"os"
	"os/signal"
	"syscall"

	"home-automation/src/cmd/mqtt-ingestor/handlers"

	mqtt "github.com/eclipse/paho.mqtt.golang"
	"github.com/jackc/pgx/v5/pgxpool"
	"go.uber.org/zap"
)

var db *pgxpool.Pool
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
	/*dbURL := os.Getenv("DATABASE_URL")
	if dbURL == "" {
		dbURL = "postgres://homeauto:homeauto@localhost:5432/homeautomation"
	}
	db, err = pgxpool.New(ctx, dbURL)
	if err != nil {
		logger.Fatal("Failed to connect to DB", zap.Error(err))
	}
	defer db.Close()*/

	// Connect to MQTT
	mqttOpts := mqtt.NewClientOptions()
	mqttOpts.AddBroker("tcp://192.168.2.27:1883")
	mqttOpts.SetClientID("mqtt-ingestor")
	mqttOpts.SetUsername("ha-mqtt")
	mqttOpts.SetPassword("yiiJYKaPq9Y7UUhG694bK5Po43o9gCuu")
	client := mqtt.NewClient(mqttOpts)

	hctx := &handlers.HandlerContext{
		Logger: logger,
		AppCtx: ctx,
		DB:     db,
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
