package handlers

import (
	"context"
	"fmt"
	"time"

	"go.uber.org/zap"
)

func (h *HandlerContext) WithRetry(ctx context.Context, maxRetries int, delay time.Duration, fn func(context.Context) error) error {
	for i := 0; i <= maxRetries; i++ {
		err := fn(ctx)
		if err == nil {
			return nil
		} else if i >= maxRetries {
			return fmt.Errorf("retries exhausted: %w", err)
		}

		h.Logger.Warn("Retry failed", zap.Int("attempt", i+1), zap.Error(err), zap.Duration("delay", delay))

		select {
		case <-time.After(delay):
		case <-ctx.Done():
			return ctx.Err()
		}
	}
	return nil
}
