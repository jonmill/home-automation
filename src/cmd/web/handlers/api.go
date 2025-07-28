package handlers

import (
	"encoding/json"
	"fmt"
	"net/http"
	"strconv"
	"time"

	dbmodels "home-automation/internal/db/models"

	"github.com/jackc/pgx/v5"
	"github.com/jackc/pgx/v5/pgxpool"
	"github.com/julienschmidt/httprouter"
	"go.uber.org/zap"
)

type ApiHandler struct {
	Db     *pgxpool.Pool
	Http   *httprouter.Router
	Logger *zap.Logger
}

type CreateBoardDTO struct {
	Name      string `json:"name"`
	OnBattery bool   `json:"on_battery"`
}

type CreateSensorDTO struct {
	Name          string `json:"name"`
	Type          string `json:"type"`
	UnitOfMeasure string `json:"unit"`
	BoardID       string `json:"board_id"`
}

func InitializeApiHandler(pool *pgxpool.Pool, router *httprouter.Router, logger *zap.Logger) *ApiHandler {

	handler := &ApiHandler{
		Db:     pool,
		Http:   router,
		Logger: logger,
	}

	logger.Info("Initializing API handler")

	router.GET("/api/boards", handler.listBoards())
	router.POST("/api/boards", handler.createBoard())
	router.PUT("/api/boards/:id", handler.updateBoard())
	router.DELETE("/api/boards/:id", handler.deleteBoard())

	router.GET("/api/sensors", handler.listSensors())
	router.POST("/api/sensors", handler.createSensor())
	router.PUT("/api/sensors/:id", handler.updateSensor())
	router.DELETE("/api/sensors/:id", handler.deleteSensor())

	logger.Info("API routes initialized", zap.String("status", "success"))

	return handler
}

func (h *ApiHandler) listBoards() httprouter.Handle {
	return func(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
		var boardId string = ps.ByName("board_id")
		var query string
		var rows pgx.Rows
		var err error
		if boardId != "" {
			query = "SELECT Id, Name, AddedAt, OnBattery FROM Boards WHERE Id = $1"
			rows, err = h.Db.Query(r.Context(), query, boardId)
		} else {
			query = "SELECT Id, Name, AddedAt, OnBattery FROM Boards"
			rows, err = h.Db.Query(r.Context(), query)
		}
		if err != nil {
			h.Logger.Error("Failed to query boards", zap.Error(err))
			http.Error(w, err.Error(), http.StatusInternalServerError)
			return
		}
		defer rows.Close()

		for rows.Next() {
			var id int
			var name string
			var addedAt time.Time
			var onBattery bool
			if err := rows.Scan(&id, &name, &addedAt, &onBattery); err == nil {
				fmt.Fprintf(w, `<option value="%d">%s</option>`, id, name)
			}
		}
	}
}

func (h *ApiHandler) createBoard() httprouter.Handle {
	return func(w http.ResponseWriter, r *http.Request, _ httprouter.Params) {
		var dto CreateBoardDTO
		if err := json.NewDecoder(r.Body).Decode(&dto); err != nil {
			h.Logger.Error("Failed to decode board", zap.Error(err))
			http.Error(w, err.Error(), http.StatusBadRequest)
			return
		}
		_, err := h.Db.Exec(r.Context(), "INSERT INTO Boards (Name, OnBattery) VALUES ($1, $2)", dto.Name, dto.OnBattery)
		if err != nil {
			h.Logger.Error("Failed to create board", zap.Error(err))
			http.Error(w, err.Error(), http.StatusInternalServerError)
			return
		}
		w.WriteHeader(http.StatusCreated)
	}
}

func (h *ApiHandler) updateBoard() httprouter.Handle {
	return func(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
		id, err := strconv.Atoi(ps.ByName("id"))
		if err != nil {
			h.Logger.Error("Invalid board ID", zap.Error(err))
			http.Error(w, "Invalid ID", http.StatusBadRequest)
			return
		}
		var b dbmodels.Board
		if err := json.NewDecoder(r.Body).Decode(&b); err != nil {
			h.Logger.Error("Failed to decode board", zap.Error(err))
			http.Error(w, err.Error(), http.StatusBadRequest)
			return
		}
		_, err = h.Db.Exec(r.Context(), "UPDATE Boards SET Name=$1, OnBattery=$2 WHERE Id=$3", b.Name, b.OnBattery, id)
		if err != nil {
			h.Logger.Error("Failed to update board", zap.Error(err))
			http.Error(w, err.Error(), http.StatusInternalServerError)
			return
		}
		w.WriteHeader(http.StatusOK)
	}
}

func (h *ApiHandler) deleteBoard() httprouter.Handle {
	return func(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
		id, err := strconv.Atoi(ps.ByName("id"))
		if err != nil {
			h.Logger.Error("Invalid board ID", zap.Error(err))
			http.Error(w, "Invalid ID", http.StatusBadRequest)
			return
		}
		_, err = h.Db.Exec(r.Context(), "DELETE FROM Boards WHERE Id=$1", id)
		if err != nil {
			h.Logger.Error("Failed to delete board", zap.Error(err))
			http.Error(w, err.Error(), http.StatusInternalServerError)
			return
		}
		w.WriteHeader(http.StatusNoContent)
	}
}

// Handlers for Sensors
func (h *ApiHandler) listSensors() httprouter.Handle {
	return func(w http.ResponseWriter, r *http.Request, _ httprouter.Params) {
		rows, err := h.Db.Query(r.Context(), "SELECT Id, Name, AddedAt, Type, Unit FROM Sensors")
		if err != nil {
			h.Logger.Error("Failed to query sensors", zap.Error(err))
			http.Error(w, err.Error(), http.StatusInternalServerError)
			return
		}
		defer rows.Close()

		var sensors []dbmodels.Sensor
		for rows.Next() {
			var s dbmodels.Sensor
			if err := rows.Scan(&s.ID, &s.Name, &s.AddedAt, &s.Type, &s.UnitOfMeasure); err != nil {
				h.Logger.Error("Failed to scan sensor", zap.Error(err))
				http.Error(w, err.Error(), http.StatusInternalServerError)
				return
			}
			sensors = append(sensors, s)
		}

		json.NewEncoder(w).Encode(sensors)
	}
}

func (h *ApiHandler) createSensor() httprouter.Handle {
	return func(w http.ResponseWriter, r *http.Request, _ httprouter.Params) {
		var dto CreateSensorDTO
		if err := json.NewDecoder(r.Body).Decode(&dto); err != nil {
			h.Logger.Error("Failed to decode sensor", zap.Error(err))
			http.Error(w, err.Error(), http.StatusBadRequest)
			return
		}
		var newSensorID int
		err := h.Db.QueryRow(r.Context(), "INSERT INTO Sensors (Name, Type, Unit) VALUES ($1, $2, $3) RETURNING Id", dto.Name, dto.Type, dto.UnitOfMeasure).Scan(&newSensorID)
		if err != nil {
			h.Logger.Error("Failed to create sensor", zap.Error(err))
			http.Error(w, err.Error(), http.StatusInternalServerError)
			return
		}
		_, err = h.Db.Exec(r.Context(), "INSERT INTO BoardSensors (BoardId, SensorID) VALUES ($1, $2)", dto.BoardID, newSensorID)
		if err != nil {
			h.Logger.Error("Failed to associate sensor with board", zap.Error(err))
			http.Error(w, err.Error(), http.StatusInternalServerError)
			return
		}
		w.WriteHeader(http.StatusCreated)
	}
}

func (h *ApiHandler) updateSensor() httprouter.Handle {
	return func(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
		id, err := strconv.Atoi(ps.ByName("id"))
		if err != nil {
			h.Logger.Error("Invalid sensor ID", zap.Error(err))
			http.Error(w, "Invalid ID", http.StatusBadRequest)
			return
		}
		var s dbmodels.Sensor
		if err := json.NewDecoder(r.Body).Decode(&s); err != nil {
			h.Logger.Error("Failed to decode sensor", zap.Error(err))
			http.Error(w, err.Error(), http.StatusBadRequest)
			return
		}
		_, err = h.Db.Exec(r.Context(), "UPDATE Sensors SET Name=$1, Type=$2, UnitOfMeasure=$3 WHERE Id=$4", s.Name, s.Type, s.UnitOfMeasure, id)
		if err != nil {
			h.Logger.Error("Failed to update sensor", zap.Error(err))
			http.Error(w, err.Error(), http.StatusInternalServerError)
			return
		}
		w.WriteHeader(http.StatusOK)
	}
}

func (h *ApiHandler) deleteSensor() httprouter.Handle {
	return func(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
		id, err := strconv.Atoi(ps.ByName("id"))
		if err != nil {
			h.Logger.Error("Invalid sensor ID", zap.Error(err))
			http.Error(w, "Invalid ID", http.StatusBadRequest)
			return
		}
		_, err = h.Db.Exec(r.Context(), "DELETE FROM Sensors WHERE Id=$1", id)
		if err != nil {
			h.Logger.Error("Failed to delete sensor", zap.Error(err))
			http.Error(w, err.Error(), http.StatusInternalServerError)
			return
		}
		w.WriteHeader(http.StatusNoContent)
	}
}
