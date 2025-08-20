using FluentMigrator;
using HomeAutomation.Models.Enums;

namespace HomeAutomation.Models.Database.Migrations;

[MigrationVersion(2025, 8, 19, true, 1, BreakingChange = false)]
public sealed class RingSensorsMigration : Migration
{
    public override void Up()
    {
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
                SerialNumber = "e65784ff-c274-4ccf-ae86-348fa1bad9a3",
                Name = "House Garage Door",
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
                SerialNumber = "e65784ff-c274-4ccf-ae86-348fa1bad9a3",
                Name = "House Garage Door",
                Type = (byte)SensorTypes.Contact,
                Unit = (byte)UnitsOfMeasure.Boolean,
                BoardSerialNumber = "e65784ff-c274-4ccf-ae86-348fa1bad9a3",
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
        // Nothing to do here
    }
}