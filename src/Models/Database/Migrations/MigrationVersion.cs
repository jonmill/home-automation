using FluentMigrator;

namespace HomeAutomation.Models.Database.Migrations;

/// <summary>
/// Enforces a specific migration versioning scheme.
/// </summary>
public sealed class MigrationVersion : MigrationAttribute
{
    public MigrationVersion(short year, byte month, byte day, bool production, byte suffix)
        : base((year * 1000000000L) + (month * 100000L) + (day * 1000L) + ((production ? 1L : 0L) * 10L) + suffix)
    {
    }
}