FROM golang:1.24.5 AS builder

WORKDIR /app

# Copy Go files
COPY go.mod go.sum ./
RUN go mod download

COPY . .

WORKDIR /app/cmd/mqtt-ingestor
RUN CGO_ENABLED=0 GOOS=linux go build -o /app/mqtt-ingestor

FROM alpine:latest
RUN apk --no-cache add ca-certificates
WORKDIR /app
COPY --from=builder /app/mqtt-ingestor .

ENTRYPOINT ["/app/mqtt-ingestor"]
