package db

import (
	"context"
	"fmt"
	"strconv"
	"sync"

	models "home-automation/src/internal/mqtt/models"

	"github.com/jackc/pgx/v5/pgxpool"
	"go.uber.org/zap"
)

// SensorCacheContext stores known sensor and board IDs for lookup before DB insert
// You can expand this to include type/unit checks, etc.
type SensorCacheContext struct {
	DB        *pgxpool.Pool
	Logger    *zap.Logger
	SensorIDs map[int]struct{} // Set of known sensor IDs
	BoardIDs  map[int]struct{} // Set of known board IDs
	mutex     sync.RWMutex
}

// NewSensorCacheContext creates and loads sensor and board IDs into memory
func NewSensorCacheContext(ctx context.Context, db *pgxpool.Pool, logger *zap.Logger) (*SensorCacheContext, error) {
	s := &SensorCacheContext{
		DB:        db,
		Logger:    logger,
		SensorIDs: make(map[int]struct{}),
		BoardIDs:  make(map[int]struct{}),
		mutex:     sync.RWMutex{},
	}
	if err := s.load(ctx); err != nil {
		return nil, fmt.Errorf("failed to load sensor cache: %w", err)
	}
	return s, nil
}

// load populates the in-memory sensor and board ID sets
func (s *SensorCacheContext) load(ctx context.Context) error {
	rows, err := s.DB.Query(ctx, "SELECT id FROM sensors")
	if err != nil {
		s.Logger.Error("Failed to query sensors", zap.Error(err))
		return err
	}
	defer rows.Close()

	s.mutex.Lock()
	for rows.Next() {
		var id int
		if err := rows.Scan(&id); err != nil {
			s.mutex.Unlock()
			s.Logger.Error("Failed to scan sensor ID", zap.Error(err))
			return err
		}
		s.SensorIDs[id] = struct{}{}
	}
	s.mutex.Unlock()

	rows, err = s.DB.Query(ctx, "SELECT id FROM boards")
	if err != nil {
		s.Logger.Error("Failed to query boards", zap.Error(err))
		return err
	}
	defer rows.Close()

	s.mutex.Lock()
	for rows.Next() {
		var id int
		if err := rows.Scan(&id); err != nil {
			s.mutex.Unlock()
			s.Logger.Error("Failed to scan board ID", zap.Error(err))
			return err
		}
		s.BoardIDs[id] = struct{}{}
	}
	s.mutex.Unlock()

	return rows.Err()
}

// SensorExists checks if a sensor ID is known
func (s *SensorCacheContext) SensorExists(id int) bool {
	s.mutex.RLock()
	defer s.mutex.RUnlock()
	_, ok := s.SensorIDs[id]
	return ok
}

// BoardExists checks if a board ID is known
func (s *SensorCacheContext) BoardExists(id int) bool {
	s.mutex.RLock()
	defer s.mutex.RUnlock()
	_, ok := s.BoardIDs[id]
	return ok
}

// SaveSensorData persists a sensor value to the database
func (s *SensorCacheContext) SaveSensorData(ctx context.Context, payload models.DataPayload) error {
	if !s.BoardExists(payload.Source) {
		s.Logger.Warn("Unknown board ID", zap.Int("board_id", payload.Source))
		return fmt.Errorf("unknown board ID: %d", payload.Source)
	}

	sensorID, err := strconv.Atoi(payload.DataID)
	if err != nil {
		s.Logger.Warn("Invalid sensor ID format", zap.String("sensor_id", payload.DataID), zap.Error(err))
		return fmt.Errorf("invalid sensor ID format: %w", err)
	}

	if !s.SensorExists(sensorID) {
		s.Logger.Warn("Unknown sensor ID", zap.Int("sensor_id", sensorID))
		return fmt.Errorf("unknown sensor ID: %d", sensorID)
	}

	_, err = s.DB.Exec(ctx,
		`INSERT INTO sensorvalues (boardid, sensorid, value, recordedat)
		 VALUES ($1, $2, $3, NOW())`,
		payload.Source, sensorID, fmt.Sprintf("%v", payload.DataValue),
	)

	if err != nil {
		s.Logger.Error("Failed to insert sensor value", zap.Error(err), zap.Int("board_id", payload.Source), zap.Int("sensor_id", sensorID))
	}
	return err
}
