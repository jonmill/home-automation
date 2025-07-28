# Build stage
FROM golang:1.24.5 AS builder

# Set working directory
WORKDIR /app

# Copy Go mod files and download deps
COPY go.mod go.sum ./
RUN go mod download

# Copy the full source code
COPY . .

# Set working directory to the web cmd folder
WORKDIR /app/cmd/web

# Build the binary
RUN CGO_ENABLED=0 GOOS=linux go build -o /app/web

# Final stage
FROM alpine:latest
RUN apk --no-cache add ca-certificates

WORKDIR /app
COPY --from=builder /app/web .

# Expose port for HTTP server
EXPOSE 8080

# Environment variable can be overridden in compose
ENV DATABASE_URL=postgres://homeauto:homeauto@localhost:5432/homeautomation

ENTRYPOINT ["/app/web"]
