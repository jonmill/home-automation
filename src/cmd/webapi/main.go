package main

import (
	"context"
	"encoding/json"
	"log"
	"net/http"
	"os"
	"strconv"

	dbmodels "home-automation/src/internal/db/models"

	"github.com/jackc/pgx/v5/pgxpool"
	"github.com/julienschmidt/httprouter"
)

func main() {
	dbURL := os.Getenv("DATABASE_URL")
	if dbURL == "" {
		dbURL = "postgres://homeauto:homeauto@localhost:5432/homeautomation"
	}

	pool, err := pgxpool.New(context.Background(), dbURL)
	if err != nil {
		log.Fatalf("Failed to connect to DB: %v", err)
	}
	defer pool.Close()

	router := httprouter.New()

	router.GET("/boards", listBoards(pool))
	router.POST("/boards", createBoard(pool))
	router.PUT("/boards/:id", updateBoard(pool))
	router.DELETE("/boards/:id", deleteBoard(pool))

	router.GET("/sensors", listSensors(pool))
	router.POST("/sensors", createSensor(pool))
	router.PUT("/sensors/:id", updateSensor(pool))
	router.DELETE("/sensors/:id", deleteSensor(pool))

	log.Println("Web API listening on :8080")
	http.ListenAndServe(":8080", router)
}

// Handlers for Boards
func listBoards(db *pgxpool.Pool) httprouter.Handle {
	return func(w http.ResponseWriter, r *http.Request, _ httprouter.Params) {
		rows, err := db.Query(r.Context(), "SELECT id, name, added_at, on_battery FROM boards")
		if err != nil {
			http.Error(w, err.Error(), http.StatusInternalServerError)
			return
		}
		defer rows.Close()

		var boards []dbmodels.Board
		for rows.Next() {
			var b dbmodels.Board
			if err := rows.Scan(&b.ID, &b.Name, &b.AddedAt, &b.OnBattery); err != nil {
				http.Error(w, err.Error(), http.StatusInternalServerError)
				return
			}
			boards = append(boards, b)
		}

		json.NewEncoder(w).Encode(boards)
	}
}

func createBoard(db *pgxpool.Pool) httprouter.Handle {
	return func(w http.ResponseWriter, r *http.Request, _ httprouter.Params) {
		var b dbmodels.Board
		if err := json.NewDecoder(r.Body).Decode(&b); err != nil {
			http.Error(w, err.Error(), http.StatusBadRequest)
			return
		}
		_, err := db.Exec(r.Context(), "INSERT INTO boards (name, on_battery) VALUES ($1, $2)", b.Name, b.OnBattery)
		if err != nil {
			http.Error(w, err.Error(), http.StatusInternalServerError)
			return
		}
		w.WriteHeader(http.StatusCreated)
	}
}

func updateBoard(db *pgxpool.Pool) httprouter.Handle {
	return func(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
		id, err := strconv.Atoi(ps.ByName("id"))
		if err != nil {
			http.Error(w, "Invalid ID", http.StatusBadRequest)
			return
		}
		var b dbmodels.Board
		if err := json.NewDecoder(r.Body).Decode(&b); err != nil {
			http.Error(w, err.Error(), http.StatusBadRequest)
			return
		}
		_, err = db.Exec(r.Context(), "UPDATE boards SET name=$1, on_battery=$2 WHERE id=$3", b.Name, b.OnBattery, id)
		if err != nil {
			http.Error(w, err.Error(), http.StatusInternalServerError)
			return
		}
		w.WriteHeader(http.StatusOK)
	}
}

func deleteBoard(db *pgxpool.Pool) httprouter.Handle {
	return func(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
		id, err := strconv.Atoi(ps.ByName("id"))
		if err != nil {
			http.Error(w, "Invalid ID", http.StatusBadRequest)
			return
		}
		_, err = db.Exec(r.Context(), "DELETE FROM boards WHERE id=$1", id)
		if err != nil {
			http.Error(w, err.Error(), http.StatusInternalServerError)
			return
		}
		w.WriteHeader(http.StatusNoContent)
	}
}

// Handlers for Sensors
func listSensors(db *pgxpool.Pool) httprouter.Handle {
	return func(w http.ResponseWriter, r *http.Request, _ httprouter.Params) {
		rows, err := db.Query(r.Context(), "SELECT id, name, added_at, sensor_type, unit_of_measure FROM sensors")
		if err != nil {
			http.Error(w, err.Error(), http.StatusInternalServerError)
			return
		}
		defer rows.Close()

		var sensors []dbmodels.Sensor
		for rows.Next() {
			var s dbmodels.Sensor
			if err := rows.Scan(&s.ID, &s.Name, &s.AddedAt, &s.Type, &s.UnitOfMeasure); err != nil {
				http.Error(w, err.Error(), http.StatusInternalServerError)
				return
			}
			sensors = append(sensors, s)
		}

		json.NewEncoder(w).Encode(sensors)
	}
}

func createSensor(db *pgxpool.Pool) httprouter.Handle {
	return func(w http.ResponseWriter, r *http.Request, _ httprouter.Params) {
		var s dbmodels.Sensor
		if err := json.NewDecoder(r.Body).Decode(&s); err != nil {
			http.Error(w, err.Error(), http.StatusBadRequest)
			return
		}
		_, err := db.Exec(r.Context(), "INSERT INTO sensors (name, type, unit_of_measure) VALUES ($1, $2, $3)", s.Name, s.Type, s.UnitOfMeasure)
		if err != nil {
			http.Error(w, err.Error(), http.StatusInternalServerError)
			return
		}
		w.WriteHeader(http.StatusCreated)
	}
}

func updateSensor(db *pgxpool.Pool) httprouter.Handle {
	return func(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
		id, err := strconv.Atoi(ps.ByName("id"))
		if err != nil {
			http.Error(w, "Invalid ID", http.StatusBadRequest)
			return
		}
		var s dbmodels.Sensor
		if err := json.NewDecoder(r.Body).Decode(&s); err != nil {
			http.Error(w, err.Error(), http.StatusBadRequest)
			return
		}
		_, err = db.Exec(r.Context(), "UPDATE sensors SET name=$1, type=$2, unit_of_measure=$3 WHERE id=$4", s.Name, s.Type, s.UnitOfMeasure, id)
		if err != nil {
			http.Error(w, err.Error(), http.StatusInternalServerError)
			return
		}
		w.WriteHeader(http.StatusOK)
	}
}

func deleteSensor(db *pgxpool.Pool) httprouter.Handle {
	return func(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
		id, err := strconv.Atoi(ps.ByName("id"))
		if err != nil {
			http.Error(w, "Invalid ID", http.StatusBadRequest)
			return
		}
		_, err = db.Exec(r.Context(), "DELETE FROM sensors WHERE id=$1", id)
		if err != nil {
			http.Error(w, err.Error(), http.StatusInternalServerError)
			return
		}
		w.WriteHeader(http.StatusNoContent)
	}
}
