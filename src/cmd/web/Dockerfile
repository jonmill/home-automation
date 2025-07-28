# Dockerfile for web
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
RUN go build -o web ./src/cmd/web

# Create a minimal runtime image
FROM alpine:latest
WORKDIR /root/
COPY --from=builder /app/web .

# Expose the necessary port
EXPOSE 8080

# Set environment variables
ENV DATABASE_URL=postgres://homeauto:homeauto@localhost:5432/homeautomation

# Command to run the binary
ENTRYPOINT ["./web"]
