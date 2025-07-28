package main

import (
	"context"
	"fmt"
	"log"
	"net/http"
	"os"
	"path/filepath"
	"runtime"

	"github.com/jackc/pgx/v5/pgxpool"
	"github.com/julienschmidt/httprouter"
	"go.uber.org/zap"

	"home-automation/cmd/web/handlers"
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

	dbContext, err = pgxpool.New(ctx, dbURL)
	if err != nil {
		logger.Fatal("Failed to connect to database", zap.Error(err))
	}
	defer dbContext.Close()

	router := httprouter.New()

	// Initialize handlers
	apiHandler := handlers.InitializeApiHandler(dbContext, router, logger)
	webHandler := handlers.InitializeWebHandler(dbContext, router, getProjectRoot(), logger)

	if apiHandler == nil || webHandler == nil {
		logger.Fatal("Failed to initialize handlers")
	}

	log.Println("Web API listening on :8080")
	http.ListenAndServe(":8080", router)
}

func getProjectRoot() string {
	_, b, _, _ := runtime.Caller(0)
	basePath := filepath.Dir(b)
	return basePath
}
