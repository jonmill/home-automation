# Dockerfile for mqtt-ingestor
FROM golang:1.24.5 AS builder

# Set working directory
WORKDIR /app

# Copy go.mod and go.sum files
COPY go.mod go.sum ./

# Download dependencies
RUN go mod download

# Copy the source code
COPY . .

# Build the binary
RUN go build -o mqtt-ingestor ./src/cmd/mqtt-ingestor

# Create a minimal runtime image
FROM alpine:latest
WORKDIR /root/
COPY --from=builder /app/mqtt-ingestor .

# Set environment variables
ENV DATABASE_URL=postgres://homeauto:homeauto@localhost:5432/homeautomation
ENV MQTT_URL=tcp://localhost:1883
ENV MQTT_PASSWORD=

# Command to run the binary
ENTRYPOINT ["./mqtt-ingestor"]
