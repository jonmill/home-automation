package handlers

import (
	"context"

	"github.com/jackc/pgx/v5/pgxpool"
	"go.uber.org/zap"
)

type HandlerContext struct {
	Logger *zap.Logger
	AppCtx context.Context
	DB     *pgxpool.Pool
}
