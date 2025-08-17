using FluentMigrator;

namespace HomeAutomation.Models.Database.Migrations;

[MigrationVersion(2025, 08, 16, true, 1)]
public sealed class SubscriptionMigration : Migration
{
    public override void Up()
    {
        Create.Table(TableNames.PushSubscriptions)
            .WithColumn("Endpoint").AsString().PrimaryKey().NotNullable()
            .WithColumn("P256dh").AsString().NotNullable()
            .WithColumn("Auth").AsString().NotNullable();
        Create.Index("IX_PushSubscriptions_Endpoint")
            .OnTable(TableNames.PushSubscriptions)
            .OnColumn("Endpoint").Ascending()
            .WithOptions().Unique();
    }

    public override void Down()
    {
        Delete.Table(TableNames.PushSubscriptions);
    }
}