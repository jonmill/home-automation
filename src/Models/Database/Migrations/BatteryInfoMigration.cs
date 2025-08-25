using FluentMigrator;

namespace HomeAutomation.Models.Database.Migrations;

[MigrationVersion(2025, 08, 24, true, 1)]
public sealed class BatteryInfoMigration : Migration
{
    public override void Up()
    {
        Create.Table(TableNames.BoardBatteryInfo)
            .WithColumn("BoardId").AsInt32().ForeignKey(TableNames.Boards, "Id").NotNullable()
            .WithColumn("BatteryLevel").AsDouble().NotNullable()
            .WithColumn("LastUpdated").AsDateTimeOffset().NotNullable();
        Create.Index("IX_BoardBatteryInfo_BoardId")
            .OnTable(TableNames.BoardBatteryInfo)
            .OnColumn("BoardId").Ascending()
            .WithOptions().Unique();
    }

    public override void Down()
    {
        Delete.Table(TableNames.BoardBatteryInfo);
    }
}