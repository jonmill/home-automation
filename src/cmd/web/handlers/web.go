package handlers

import (
	"html/template"
	"net/http"
	"os"
	"path/filepath"

	dbmodels "home-automation/src/internal/db/models"

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
		h.renderIndexWithPartial(w)
	}
}

func (h *WebHandler) renderIndexWithPartial(w http.ResponseWriter) {
	contentBytes, err := os.ReadFile(h.PartialsPath)
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

func (h *WebHandler) renderBoardsTable(w http.ResponseWriter, boards []dbmodels.Board) {
	tmplPath := filepath.Join(h.PartialsPath, "table_boards.tmpl")

	tmpl, err := template.ParseFiles(tmplPath)
	if err != nil {
		http.Error(w, "Template error", http.StatusInternalServerError)
		return
	}

	w.Header().Set("Content-Type", "text/html")
	tmpl.Execute(w, boards)
}

func (h *WebHandler) renderSensorsTable(w http.ResponseWriter, sensors []dbmodels.Sensor) {
	tmplPath := filepath.Join(h.PartialsPath, "table_sensors.tmpl")

	tmpl, err := template.ParseFiles(tmplPath)
	if err != nil {
		http.Error(w, "Template error", http.StatusInternalServerError)
		return
	}

	w.Header().Set("Content-Type", "text/html")
	tmpl.Execute(w, sensors)
}
