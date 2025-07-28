CREATE TABLE Boards (
    Id SERIAL PRIMARY KEY,
    Name TEXT NOT NULL,
    AddedAt TIMESTAMPTZ NOT NULL DEFAULT now(),
    OnBattery BOOLEAN NOT NULL DEFAULT false,
    IsDeleted BOOLEAN NOT NULL DEFAULT false,
    DeletedOn TIMESTAMPTZ
);

CREATE TABLE Sensors (
    Id SERIAL PRIMARY KEY,
    Name TEXT NOT NULL,
    AddedAt TIMESTAMPTZ NOT NULL DEFAULT now(),
    Type INT NOT NULL,
    Unit INT NOT NULL,
    IsDeleted BOOLEAN NOT NULL DEFAULT false,
    DeletedOn TIMESTAMPTZ
);

CREATE TABLE BoardSensors (
    BoardId INTEGER NOT NULL REFERENCES Boards(Id) ON DELETE RESTRICT,
    SensorId INTEGER NOT NULL REFERENCES Sensors(Id) ON DELETE RESTRICT,
    PRIMARY KEY (BoardId, SensorId)
);

CREATE TABLE SensorValues (
    BoardId INTEGER NOT NULL REFERENCES Boards(Id) ON DELETE RESTRICT,
    SensorId INTEGER NOT NULL REFERENCES Sensors(Id) ON DELETE RESTRICT,
    RecordedAt TIMESTAMPTZ NOT NULL DEFAULT now(),
    Value TEXT NOT NULL,
    PRIMARY KEY (BoardId, SensorId, RecordedAt)
);

CREATE TABLE LogEntries (
    Id BIGSERIAL PRIMARY KEY,
    BoardId INTEGER NOT NULL REFERENCES Boards(Id) ON DELETE RESTRICT,
    Message TEXT NOT NULL,
    Level INTEGER NOT NULL,
    Tag TEXT,
    LoggedAt TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE INDEX idx_sensor_values_recorded_at ON SensorValues (RecordedAt);
CREATE INDEX idx_log_entries_logged_at ON LogEntries (LoggedAt);

INSERT INTO Boards (Id, Name, OnBattery) VALUES
(1, 'Home Security', false),
(2, 'Daylight Tracker', false),
(3, 'Car Garage Door', false),
(4, 'Front House Environment', false),
(7, 'Nora Room Air Quality', false);

INSERT INTO Sensors (Id, Name, Type, Unit) VALUES
(1, 'Front Door', 6, 5),
(2, 'Interior Garage Door', 6, 5),
(3, 'Dining Room Door', 6, 5),
(4, 'Stair Side Door', 6, 5),
(5, 'Chimney Side Door', 6, 5),
(6, 'TV Room Door', 6, 5),
(7, 'Car Garage Door', 6, 5),
(8, 'Storage Garage Door', 6, 5),
(9, 'Downstairs Air Temp', 0, 0),
(10, 'Downstairs Air Pressure', 3, 3),
(11, 'Downstairs Air Humidity', 1, 1),
(12, 'Daylight Lux', 2, 2),
(13, 'Nora Room Organic Compounds', 7, 6),
(14, 'Nora Room Air Temperature', 0, 0),
(15, 'Nora Room Air Pressure', 3, 3),
(16, 'Nora Room Air Humidity', 1, 1),
(17, 'Front House Environment Temperature', 0, 0),
(18, 'Front House Environment Humidity', 1, 1),
(19, 'Front House Environment Pressure', 3, 3),
(20, 'Garage Air Temperature', 0, 0);

INSERT INTO BoardSensors (BoardId, SensorId) VALUES
(1, 1),
(1, 2),
(1, 3),
(1, 4),
(1, 5),
(1, 6),
(2, 12),
(3, 7),
(3, 8),
(4, 18),
(4, 19),
(4, 20),
(4, 21),
(7, 13),
(7, 14),
(7, 15),
(7, 16);