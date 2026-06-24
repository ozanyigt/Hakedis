using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Persistence.Contexts;

#nullable disable

namespace Persistence.Migrations;

[DbContext(typeof(BaseDbContext))]
[Migration("20260623130000_MeasurementUnitEnum")]
public partial class MeasurementUnitEnum : Migration
{
    private const string MapUnitSql =
        """
        CASE
            WHEN Unit IN (N'm²', N'm2', N'M2', N'M²', N'metre kare', N'metrekare', N'1') THEN 1
            WHEN Unit IN (N'm³', N'm3', N'M3', N'M³', N'metre küp', N'metrekup', N'2') THEN 2
            WHEN Unit IN (N'm', N'mt', N'metre', N'M', N'3') THEN 3
            WHEN Unit IN (N'kg', N'KG', N'kilogram', N'4') THEN 4
            WHEN Unit IN (N'ton', N'TON', N't', N'5') THEN 5
            WHEN Unit IN (N'adet', N'ADET', N'Adet', N'6') THEN 6
            WHEN Unit IN (N'takım', N'takim', N'Takım', N'TAKIM', N'7') THEN 7
            ELSE 1
        END
        """;

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        ConvertTable(migrationBuilder, "ContractItems");
        ConvertTable(migrationBuilder, "MetrajResults");
        ConvertTable(migrationBuilder, "MetrajRuleTemplates");
    }

    private static void ConvertTable(MigrationBuilder migrationBuilder, string tableName)
    {
        migrationBuilder.AddColumn<int>(
            name: "Unit_New",
            table: tableName,
            type: "int",
            nullable: false,
            defaultValue: 1);

        migrationBuilder.Sql($"UPDATE {tableName} SET Unit_New = {MapUnitSql};");

        migrationBuilder.DropColumn(name: "Unit", table: tableName);

        migrationBuilder.RenameColumn(
            name: "Unit_New",
            table: tableName,
            newName: "Unit");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        RevertTable(migrationBuilder, "ContractItems");
        RevertTable(migrationBuilder, "MetrajResults");
        RevertTable(migrationBuilder, "MetrajRuleTemplates");
    }

    private static void RevertTable(MigrationBuilder migrationBuilder, string tableName)
    {
        migrationBuilder.AddColumn<string>(
            name: "Unit_Old",
            table: tableName,
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "m²");

        migrationBuilder.Sql(
            $"""
            UPDATE {tableName}
            SET Unit_Old = CASE Unit
                WHEN 1 THEN N'm²'
                WHEN 2 THEN N'm³'
                WHEN 3 THEN N'm'
                WHEN 4 THEN N'kg'
                WHEN 5 THEN N'ton'
                WHEN 6 THEN N'adet'
                WHEN 7 THEN N'takım'
                ELSE N'm²'
            END
            """);

        migrationBuilder.DropColumn(name: "Unit", table: tableName);

        migrationBuilder.RenameColumn(
            name: "Unit_Old",
            table: tableName,
            newName: "Unit");
    }
}
