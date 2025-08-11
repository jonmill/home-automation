using FluentMigrator;

namespace HomeAutomation.Models.Database.Migrations;

[MigrationVersion(2025, 08, 09, true, 1)]
public sealed class InitialMigration : Migration
{
    public override void Up()
    {
        Create.Table(TableNames.Boards)
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("name").AsString().NotNullable()
            .WithColumn("added_at").AsDateTimeOffset().NotNullable()
            .WithColumn("on_battery").AsBoolean().NotNullable()
            .WithColumn("is_deleted").AsBoolean().NotNullable()
            .WithColumn("deleted_on").AsDateTimeOffset().Nullable();
        Create.Table(TableNames.Heartbeats)
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("board_id").AsInt32().NotNullable()
            .WithColumn("timestamp").AsDateTimeOffset().NotNullable()
            .WithColumn("next_expected_heartbeat").AsDateTimeOffset().NotNullable();
        Create.ForeignKey("fk_heartbeats_boards")
            .FromTable(TableNames.Heartbeats).ForeignColumn("board_id")
            .ToTable(TableNames.Boards).PrimaryColumn("id");
        Create.Index("idx_heartbeats_board_id")
            .OnTable(TableNames.Heartbeats)
            .OnColumn("board_id").Ascending();
        Create.Table(TableNames.Sensors)
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("name").AsString().NotNullable()
            .WithColumn("type").AsByte().NotNullable()
            .WithColumn("unit").AsByte().NotNullable()
            .WithColumn("board_id").AsInt32().NotNullable()
            .WithColumn("added_at").AsDateTimeOffset().NotNullable()
            .WithColumn("is_deleted").AsBoolean().NotNullable()
            .WithColumn("deleted_on").AsDateTimeOffset().Nullable();
        Create.ForeignKey("fk_sensors_boards")
            .FromTable(TableNames.Sensors).ForeignColumn("board_id")
            .ToTable(TableNames.Boards).PrimaryColumn("id");
        Create.Table(TableNames.SensorValues)
            .WithColumn("board_id").AsInt32().PrimaryKey().NotNullable()
            .WithColumn("sensor_id").AsInt32().PrimaryKey().NotNullable()
            .WithColumn("value").AsString().NotNullable()
            .WithColumn("timestamp").AsDateTimeOffset().NotNullable();
        Create.ForeignKey("fk_sensorvalues_boards")
            .FromTable(TableNames.SensorValues).ForeignColumn("board_id")
            .ToTable(TableNames.Boards).PrimaryColumn("id");
        Create.ForeignKey("fk_sensorvalues_sensors")
            .FromTable(TableNames.SensorValues).ForeignColumn("sensor_id")
            .ToTable(TableNames.Sensors).PrimaryColumn("id");
        Create.Index("idx_sensorvalues_boards_boardid")
            .OnTable(TableNames.SensorValues)
            .OnColumn("board_id").Ascending();
        Create.Index("idx_sensorvalues_board_sensorid")
            .OnTable(TableNames.SensorValues)
            .OnColumn("sensor_id").Ascending();
        Create.Table(TableNames.LogEntries)
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("timestamp").AsDateTimeOffset().NotNullable()
            .WithColumn("board_id").AsInt32().NotNullable()
            .WithColumn("level").AsInt32().NotNullable()
            .WithColumn("message").AsString().NotNullable()
            .WithColumn("tag").AsString().Nullable();
        Create.ForeignKey("fk_logentries_boards")
            .FromTable(TableNames.LogEntries).ForeignColumn("board_id")
            .ToTable(TableNames.Boards).PrimaryColumn("id");
        Create.Index("idx_logentries_board_timestamp")
            .OnTable(TableNames.LogEntries)
            .OnColumn("timestamp").Descending();
        Create.Index("idx_logentries_board_id")
            .OnTable(TableNames.LogEntries)
            .OnColumn("board_id").Ascending();

        #warning ADD BOARDS AND SENSORS 
    }

    public override void Down()
    {
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