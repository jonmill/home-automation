using FluentMigrator;
using HomeAutomation.Models.Enums;

namespace HomeAutomation.Models.Database.Migrations;

[MigrationVersion(2025, 08, 26, true, 1)]
public sealed class InitialMigration : Migration
{
    public override void Up()
    {
        Create.Table(TableNames.Boards)
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("SerialNumber").AsString().NotNullable()
            .WithColumn("Name").AsString().NotNullable()
            .WithColumn("AddedAt").AsDateTimeOffset().NotNullable()
            .WithColumn("OnBattery").AsBoolean().NotNullable()
            .WithColumn("IsDeleted").AsBoolean().NotNullable()
            .WithColumn("DeletedOn").AsDateTimeOffset().Nullable();
        Create.UniqueConstraint("uc_boards_serialnumber")
            .OnTable(TableNames.Boards)
            .Columns("SerialNumber");
        Create.Table(TableNames.Heartbeats)
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("BoardSerialNumber").AsString().NotNullable()
            .WithColumn("Timestamp").AsDateTimeOffset().NotNullable()
            .WithColumn("NextExpectedHeartbeat").AsDateTimeOffset().NotNullable();
        Create.ForeignKey("fk_heartbeats_boards")
            .FromTable(TableNames.Heartbeats).ForeignColumn("BoardSerialNumber")
            .ToTable(TableNames.Boards).PrimaryColumn("SerialNumber");
        Create.Index("idx_heartbeats_board_serialnumber")
            .OnTable(TableNames.Heartbeats)
            .OnColumn("BoardSerialNumber").Ascending();
        Create.Table(TableNames.Sensors)
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("SerialNumber").AsString().NotNullable()
            .WithColumn("Name").AsString().NotNullable()
            .WithColumn("Type").AsByte().NotNullable()
            .WithColumn("Unit").AsByte().NotNullable()
            .WithColumn("BoardSerialNumber").AsString().NotNullable()
            .WithColumn("AddedAt").AsDateTimeOffset().NotNullable()
            .WithColumn("IsDeleted").AsBoolean().NotNullable()
            .WithColumn("DeletedOn").AsDateTimeOffset().Nullable();
        Create.UniqueConstraint("uc_sensors_serialnumber")
            .OnTable(TableNames.Sensors)
            .Columns("SerialNumber");
        Create.ForeignKey("fk_sensors_boards")
            .FromTable(TableNames.Sensors).ForeignColumn("BoardSerialNumber")
            .ToTable(TableNames.Boards).PrimaryColumn("SerialNumber");
        Create.Table(TableNames.SensorValues)
            .WithColumn("BoardSerialNumber").AsString().NotNullable()
            .WithColumn("SensorSerialNumber").AsString().NotNullable()
            .WithColumn("Value").AsString().NotNullable()
            .WithColumn("Timestamp").AsDateTimeOffset().NotNullable();
        Create.ForeignKey("fk_sensorvalues_boards")
            .FromTable(TableNames.SensorValues).ForeignColumn("BoardSerialNumber")
            .ToTable(TableNames.Boards).PrimaryColumn("SerialNumber");
        Create.ForeignKey("fk_sensorvalues_sensors")
            .FromTable(TableNames.SensorValues).ForeignColumn("SensorSerialNumber")
            .ToTable(TableNames.Sensors).PrimaryColumn("SerialNumber");
        Create.Index("idx_sensorvalues_boards_boardserialnumber")
            .OnTable(TableNames.SensorValues)
            .OnColumn("BoardSerialNumber").Ascending();
        Create.Index("idx_sensorvalues_board_sensorserialnumber")
            .OnTable(TableNames.SensorValues)
            .OnColumn("SensorSerialNumber").Ascending();
        Create.Table(TableNames.LogEntries)
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("Timestamp").AsDateTimeOffset().NotNullable()
            .WithColumn("BoardSerialNumber").AsString().NotNullable()
            .WithColumn("Level").AsInt32().NotNullable()
            .WithColumn("Message").AsString().NotNullable()
            .WithColumn("Tag").AsString().Nullable();
        Create.ForeignKey("fk_logentries_boards")
            .FromTable(TableNames.LogEntries).ForeignColumn("BoardSerialNumber")
            .ToTable(TableNames.Boards).PrimaryColumn("SerialNumber");
        Create.Index("idx_logentries_board_timestamp")
            .OnTable(TableNames.LogEntries)
            .OnColumn("Timestamp").Descending();
        Create.Index("idx_logentries_board_serialnumber")
            .OnTable(TableNames.LogEntries)
            .OnColumn("BoardSerialNumber").Ascending();
        Create.Table(TableNames.PushSubscriptions)
            .WithColumn("Endpoint").AsString().PrimaryKey().NotNullable()
            .WithColumn("P256dh").AsString().NotNullable()
            .WithColumn("Auth").AsString().NotNullable();
        Create.Index("IX_PushSubscriptions_Endpoint")
            .OnTable(TableNames.PushSubscriptions)
            .OnColumn("Endpoint").Ascending()
            .WithOptions().Unique();
        Create.Table(TableNames.BoardBatteryInfo)
            .WithColumn("BoardId").AsInt32().ForeignKey(TableNames.Boards, "Id").NotNullable()
            .WithColumn("BatteryLevel").AsDouble().NotNullable()
            .WithColumn("LastUpdated").AsDateTimeOffset().NotNullable();
        Create.Index("IX_BoardBatteryInfo_BoardId")
            .OnTable(TableNames.BoardBatteryInfo)
            .OnColumn("BoardId").Ascending();

        Insert.IntoTable(TableNames.Boards)
            .Row(new { SerialNumber = "1", Name = "Home Security", OnBattery = false, AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "2", Name = "Daylight Tracker", OnBattery = false, AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "3", Name = "Car Garage Door", OnBattery = false, AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "4", Name = "Front House Environment", OnBattery = false, AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "5", Name = "Downstairs Air Quality", OnBattery = false, AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "6", Name = "Nora Room Air Quality", OnBattery = false, AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "7", Name = "Garage Environment", OnBattery = false, AddedAt = DateTimeOffset.UtcNow, IsDeleted = false });

        Insert.IntoTable(TableNames.Sensors)
            .Row(new { SerialNumber = "1", Name = "Front Door", Type = (byte)SensorTypes.Contact, Unit = (byte)UnitsOfMeasure.Boolean, BoardSerialNumber = "1", AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "2", Name = "Interior Garage Door", Type = (byte)SensorTypes.Contact, Unit = (byte)UnitsOfMeasure.Boolean, BoardSerialNumber = "1", AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "3", Name = "Dining Room Door", Type = (byte)SensorTypes.Contact, Unit = (byte)UnitsOfMeasure.Boolean, BoardSerialNumber = "1", AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "4", Name = "Stair Side Door", Type = (byte)SensorTypes.Contact, Unit = (byte)UnitsOfMeasure.Boolean, BoardSerialNumber = "1", AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "5", Name = "Chimney Side Door", Type = (byte)SensorTypes.Contact, Unit = (byte)UnitsOfMeasure.Boolean, BoardSerialNumber = "1", AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "6", Name = "TV Room Door", Type = (byte)SensorTypes.Contact, Unit = (byte)UnitsOfMeasure.Boolean, BoardSerialNumber = "1", AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "7", Name = "Car Garage Door", Type = (byte)SensorTypes.Contact, Unit = (byte)UnitsOfMeasure.Boolean, BoardSerialNumber = "3", AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "8", Name = "Storage Garage Door", Type = (byte)SensorTypes.Contact, Unit = (byte)UnitsOfMeasure.Boolean, BoardSerialNumber = "3", AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "9", Name = "Downstairs Air Temp", Type = (byte)SensorTypes.Temperature, Unit = (byte)UnitsOfMeasure.Celsius, BoardSerialNumber = "5", AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "10", Name = "Downstairs Air Pressure", Type = (byte)SensorTypes.Pressure, Unit = (byte)UnitsOfMeasure.Pascal, BoardSerialNumber = "5", AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "11", Name = "Downstairs Air Humidity", Type = (byte)SensorTypes.Humidity, Unit = (byte)UnitsOfMeasure.Percentage, BoardSerialNumber = "5", AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "12", Name = "Daylight Lux", Type = (byte)SensorTypes.Light, Unit = (byte)UnitsOfMeasure.Lux, BoardSerialNumber = "2", AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "13", Name = "Nora Room Air Quality", Type = (byte)SensorTypes.AirQuality, Unit = (byte)UnitsOfMeasure.AirQualityIndex, BoardSerialNumber = "6", AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "14", Name = "Nora Room Air Temperature", Type = (byte)SensorTypes.Temperature, Unit = (byte)UnitsOfMeasure.Celsius, BoardSerialNumber = "6", AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "15", Name = "Nora Room Air Pressure", Type = (byte)SensorTypes.Pressure, Unit = (byte)UnitsOfMeasure.Pascal, BoardSerialNumber = "6", AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "16", Name = "Nora Room Air Humidity", Type = (byte)SensorTypes.Humidity, Unit = (byte)UnitsOfMeasure.Percentage, BoardSerialNumber = "6", AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "17", Name = "Front House Environment Temperature", Type = (byte)SensorTypes.Temperature, Unit = (byte)UnitsOfMeasure.Celsius, BoardSerialNumber = "7", AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "18", Name = "Front House Environment Humidity", Type = (byte)SensorTypes.Humidity, Unit = (byte)UnitsOfMeasure.Percentage, BoardSerialNumber = "4", AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "19", Name = "Front House Environment Pressure", Type = (byte)SensorTypes.Pressure, Unit = (byte)UnitsOfMeasure.Pascal, BoardSerialNumber = "4", AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "20", Name = "BME Front House Air Temperature", Type = (byte)SensorTypes.Temperature, Unit = (byte)UnitsOfMeasure.Celsius, BoardSerialNumber = "4", AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "21", Name = "Front House Air Quality", Type = (byte)SensorTypes.AirQuality, Unit = (byte)UnitsOfMeasure.AirQualityIndex, BoardSerialNumber = "4", AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "22", Name = "Front House Air Quality Confidence", Type = (byte)SensorTypes.AirQualityConfidence, Unit = (byte)UnitsOfMeasure.BoschAirQualityConfidence, BoardSerialNumber = "4", AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "23", Name = "Garage Inside Environment Temperature", Type = (byte)SensorTypes.Temperature, Unit = (byte)UnitsOfMeasure.Celsius, BoardSerialNumber = "7", AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "24", Name = "Garage Inside Environment Humidity", Type = (byte)SensorTypes.Humidity, Unit = (byte)UnitsOfMeasure.Percentage, BoardSerialNumber = "7", AddedAt = DateTimeOffset.UtcNow, IsDeleted = false })
            .Row(new { SerialNumber = "25", Name = "Garage Inside Environment Pressure", Type = (byte)SensorTypes.Pressure, Unit = (byte)UnitsOfMeasure.Pascal, BoardSerialNumber = "7", AddedAt = DateTimeOffset.UtcNow, IsDeleted = false });



        Insert.IntoTable(TableNames.Boards)
            .Row(new
            {
                SerialNumber = "ae698f45-bb4b-478d-9a6b-438f334b596b",
                Name = "Kimyen Office Window",
                AddedAt = DateTimeOffset.UtcNow,
                OnBattery = true,
                IsDeleted = false,
            })
            .Row(new
            {
                SerialNumber = "d13f7da9-ead4-4d8d-9812-ae57b98e5491",
                Name = "Jon Office Window",
                AddedAt = DateTimeOffset.UtcNow,
                OnBattery = true,
                IsDeleted = false,
            })
            .Row(new
            {
                SerialNumber = "cd4ed731-1f88-4f2a-9ae8-5c4fe90fd71d",
                Name = "TV Room Chimney Window",
                AddedAt = DateTimeOffset.UtcNow,
                OnBattery = true,
                IsDeleted = false,
            })
            .Row(new
            {
                SerialNumber = "a3ba703d-2e22-4b32-ae15-ef3d12ea9014",
                Name = "TV Room Interior Window",
                AddedAt = DateTimeOffset.UtcNow,
                OnBattery = true,
                IsDeleted = false,
            })
            .Row(new
            {
                SerialNumber = "1e4e05b8-eb43-4877-8a8c-7ed87cbf74b7",
                Name = "Downstairs Bathroom Window",
                AddedAt = DateTimeOffset.UtcNow,
                OnBattery = true,
                IsDeleted = false,
            })
            .Row(new
            {
                SerialNumber = "d54ccc81-3601-4b7b-b2ca-191687f70237",
                Name = "Garage Window",
                AddedAt = DateTimeOffset.UtcNow,
                OnBattery = true,
                IsDeleted = false,
            })
            .Row(new
            {
                SerialNumber = "b50ca877-31fa-4fe6-9026-e250e17558e5",
                Name = "Kitchen Window",
                AddedAt = DateTimeOffset.UtcNow,
                OnBattery = true,
                IsDeleted = false,
            })
            .Row(new
            {
                SerialNumber = "01c72dde-2213-48dc-b548-c2b0ed215dff",
                Name = "Nora Bedroom Window",
                AddedAt = DateTimeOffset.UtcNow,
                OnBattery = true,
                IsDeleted = false,
            })
            .Row(new
            {
                SerialNumber = "2b07e27c-9481-4915-b6c0-5bee2aab6f3d",
                Name = "Guest Bedroom Window",
                AddedAt = DateTimeOffset.UtcNow,
                OnBattery = true,
                IsDeleted = false,
            });
        Insert.IntoTable(TableNames.Sensors)
            .Row(new
            {
                SerialNumber = "ae698f45-bb4b-478d-9a6b-438f334b596b",
                Name = "Kimyen Office Window",
                Type = (byte)SensorTypes.Contact,
                Unit = (byte)UnitsOfMeasure.Boolean,
                BoardSerialNumber = "ae698f45-bb4b-478d-9a6b-438f334b596b",
                AddedAt = DateTimeOffset.UtcNow,
                IsDeleted = false,
            })
            .Row(new
            {
                SerialNumber = "d13f7da9-ead4-4d8d-9812-ae57b98e5491",
                Name = "Jon Office Window",
                Type = (byte)SensorTypes.Contact,
                Unit = (byte)UnitsOfMeasure.Boolean,
                BoardSerialNumber = "d13f7da9-ead4-4d8d-9812-ae57b98e5491",
                AddedAt = DateTimeOffset.UtcNow,
                IsDeleted = false,
            })
            .Row(new
            {
                SerialNumber = "cd4ed731-1f88-4f2a-9ae8-5c4fe90fd71d",
                Name = "TV Room Chimney Window",
                Type = (byte)SensorTypes.Contact,
                Unit = (byte)UnitsOfMeasure.Boolean,
                BoardSerialNumber = "cd4ed731-1f88-4f2a-9ae8-5c4fe90fd71d",
                AddedAt = DateTimeOffset.UtcNow,
                IsDeleted = false,
            })
            .Row(new
            {
                SerialNumber = "a3ba703d-2e22-4b32-ae15-ef3d12ea9014",
                Name = "TV Room Interior Window",
                Type = (byte)SensorTypes.Contact,
                Unit = (byte)UnitsOfMeasure.Boolean,
                BoardSerialNumber = "a3ba703d-2e22-4b32-ae15-ef3d12ea9014",
                AddedAt = DateTimeOffset.UtcNow,
                IsDeleted = false,
            })
            .Row(new
            {
                SerialNumber = "1e4e05b8-eb43-4877-8a8c-7ed87cbf74b7",
                Name = "Downstairs Bathroom Window",
                Type = (byte)SensorTypes.Contact,
                Unit = (byte)UnitsOfMeasure.Boolean,
                BoardSerialNumber = "1e4e05b8-eb43-4877-8a8c-7ed87cbf74b7",
                AddedAt = DateTimeOffset.UtcNow,
                IsDeleted = false,
            })
            .Row(new
            {
                SerialNumber = "d54ccc81-3601-4b7b-b2ca-191687f70237",
                Name = "Garage Window",
                Type = (byte)SensorTypes.Contact,
                Unit = (byte)UnitsOfMeasure.Boolean,
                BoardSerialNumber = "d54ccc81-3601-4b7b-b2ca-191687f70237",
                AddedAt = DateTimeOffset.UtcNow,
                IsDeleted = false,
            })
            .Row(new
            {
                SerialNumber = "b50ca877-31fa-4fe6-9026-e250e17558e5",
                Name = "Kitchen Window",
                Type = (byte)SensorTypes.Contact,
                Unit = (byte)UnitsOfMeasure.Boolean,
                BoardSerialNumber = "b50ca877-31fa-4fe6-9026-e250e17558e5",
                AddedAt = DateTimeOffset.UtcNow,
                IsDeleted = false,
            })
            .Row(new
            {
                SerialNumber = "01c72dde-2213-48dc-b548-c2b0ed215dff",
                Name = "Nora Bedroom Window",
                Type = (byte)SensorTypes.Contact,
                Unit = (byte)UnitsOfMeasure.Boolean,
                BoardSerialNumber = "01c72dde-2213-48dc-b548-c2b0ed215dff",
                AddedAt = DateTimeOffset.UtcNow,
                IsDeleted = false,
            })
            .Row(new
            {
                SerialNumber = "2b07e27c-9481-4915-b6c0-5bee2aab6f3d",
                Name = "Guest Bedroom Window",
                Type = (byte)SensorTypes.Contact,
                Unit = (byte)UnitsOfMeasure.Boolean,
                BoardSerialNumber = "2b07e27c-9481-4915-b6c0-5bee2aab6f3d",
                AddedAt = DateTimeOffset.UtcNow,
                IsDeleted = false,
            });
    }

    public override void Down()
    {
        Delete.Index("IX_BoardBatteryInfo_BoardId").OnTable(TableNames.BoardBatteryInfo);
        Delete.Table(TableNames.BoardBatteryInfo);
        Delete.Index("IX_PushSubscriptions_Endpoint").OnTable(TableNames.PushSubscriptions);
        Delete.Table(TableNames.PushSubscriptions);
        Delete.Index("idx_logentries_board_id").OnTable(TableNames.LogEntries);
        Delete.Index("idx_logentries_board_timestamp").OnTable(TableNames.LogEntries);
        Delete.ForeignKey("fk_logentries_boards").OnTable(TableNames.LogEntries);
        Delete.Table(TableNames.LogEntries);
        Delete.Index("idx_sensorvalues_boards_boardid").OnTable(TableNames.SensorValues);
        Delete.Index("idx_sensorvalues_boards_sensorid").OnTable(TableNames.SensorValues);
        Delete.ForeignKey("fk_sensorvalues_boards").OnTable(TableNames.SensorValues);
        Delete.ForeignKey("fk_sensorvalues_sensors").OnTable(TableNames.SensorValues);
        Delete.Table(TableNames.SensorValues);
        Delete.ForeignKey("fk_sensors_boards").OnTable(TableNames.Sensors);
        Delete.Table(TableNames.Sensors);
        Delete.Index("idx_heartbeats_board_id").OnTable(TableNames.Heartbeats);
        Delete.ForeignKey("fk_heartbeats_boards").OnTable(TableNames.Heartbeats);
        Delete.Table(TableNames.Heartbeats);
        Delete.Table(TableNames.Boards);
    }
}