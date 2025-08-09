package handlers

import (
	"html/template"
	"net/http"
	"os"
	"path/filepath"

	dbmodels "home-automation/internal/db/models"

	"github.com/jackc/pgx/v5/pgxpool"
	"github.com/julienschmidt/httprouter"
	"go.uber.org/zap"
)

type WebHandler struct {
	Db           *pgxpool.Pool
	Http         *httprouter.Router
	Logger       *zap.Logger
	IndexPath    string
	PartialsPath string
	StaticPath   string
}

func InitializeWebHandler(pool *pgxpool.Pool, router *httprouter.Router, projectRoot string, logger *zap.Logger) *WebHandler {

	handler := &WebHandler{
		Db:           pool,
		Http:         router,
		Logger:       logger,
		IndexPath:    filepath.Join(projectRoot, "templates", "index.html"),
		PartialsPath: filepath.Join(projectRoot, "templates", "partials"),
		StaticPath:   filepath.Join(projectRoot, "static"),
	}

	logger.Info("Initializing Web handlers")

	// Serve static assets like CSS, JS
	fs := http.FileServer(http.Dir(handler.StaticPath))
	router.Handler("GET", "/static/*filepath", http.StripPrefix("/static/", fs))

	// Define HTMX-aware route handlers
	router.GET("/dashboard", func(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
		handler.servePartialOrFull(w, r, "dashboard.html")
	})
	router.GET("/boards", func(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
		handler.servePartialOrFull(w, r, "boards.html")
	})
	router.GET("/sensors", func(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
		handler.servePartialOrFull(w, r, "sensors.html")
	})
	router.GET("/datafeed", func(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
		handler.servePartialOrFull(w, r, "datafeed.html")
	})
	router.GET("/sensors/table", func(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
		handler.renderSensorsTable(w, r)
	})
	router.GET("/boards/table", func(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
		handler.renderBoardsTable(w, r)
	})

	router.GET("/index.html", func(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
		http.ServeFile(w, r, handler.IndexPath)
	})

	// Catch-all root or unknown route handler
	router.GET("/", func(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
		path := r.URL.Path
		if path == "/" {
			http.ServeFile(w, r, handler.IndexPath)
			return
		} else {
			w.WriteHeader(http.StatusNotFound)
			http.ServeFile(w, r, filepath.Join(handler.PartialsPath, "notfound.html"))
		}
	})

	logger.Info("Web routes initialized", zap.String("status", "success"))

	return handler
}

func (h *WebHandler) servePartialOrFull(w http.ResponseWriter, r *http.Request, templateName string) {
	isHTMX := r.Header.Get("HX-Request") == "true"
	if isHTMX {
		http.ServeFile(w, r, filepath.Join(h.PartialsPath, templateName))
	} else {
		h.renderIndexWithPartial(w, templateName)
	}
}

func (h *WebHandler) renderIndexWithPartial(w http.ResponseWriter, templateName string) {
	partialPath := filepath.Join(h.PartialsPath, templateName)
	contentBytes, err := os.ReadFile(partialPath)
	if err != nil {
		http.Error(w, "Could not load page", http.StatusInternalServerError)
		return
	}

	tmpl, err := template.ParseFiles(h.IndexPath)
	if err != nil {
		http.Error(w, "Template parse error", http.StatusInternalServerError)
		return
	}

	data := struct {
		MainContent template.HTML
	}{
		MainContent: template.HTML(contentBytes),
	}

	tmpl.Execute(w, data)
}

func (h *WebHandler) renderBoardsTable(w http.ResponseWriter, r *http.Request) {
	tmplPath := filepath.Join(h.PartialsPath, "table_boards.html")

	tmpl, err := template.ParseFiles(tmplPath)
	if err != nil {
		http.Error(w, "Template error", http.StatusInternalServerError)
		return
	}

	rows, err := h.Db.Query(r.Context(), "SELECT id, name, addedat, onbattery FROM boards WHERE isdeleted = false")
	if err != nil {
		h.Logger.Error("Failed to query boards", zap.Error(err))
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}
	defer rows.Close()

	var boards []dbmodels.Board
	for rows.Next() {
		var b dbmodels.Board
		if err := rows.Scan(&b.ID, &b.Name, &b.AddedAt, &b.OnBattery); err != nil {
			h.Logger.Error("Failed to scan board", zap.Error(err))
			http.Error(w, err.Error(), http.StatusInternalServerError)
			return
		}
		boards = append(boards, b)
	}

	w.Header().Set("Content-Type", "text/html")
	tmpl.Execute(w, boards)
}

func (h *WebHandler) renderSensorsTable(w http.ResponseWriter, r *http.Request) {
	tmplPath := filepath.Join(h.PartialsPath, "table_sensors.html")

	tmpl, err := template.ParseFiles(tmplPath)
	if err != nil {
		http.Error(w, "Template error", http.StatusInternalServerError)
		return
	}

	rows, err := h.Db.Query(r.Context(), "SELECT id, name, addedat, type, unit FROM sensors WHERE isdeleted = false")
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
		s.Type = dbmodels.SensorType(s.Type)
		s.UnitOfMeasure = dbmodels.UnitOfMeasure(s.UnitOfMeasure)
		sensors = append(sensors, s)
	}

	w.Header().Set("Content-Type", "text/html")
	tmpl.Execute(w, sensors)
}
