using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Persistence.Contexts;

#nullable disable

namespace Persistence.Migrations;

[DbContext(typeof(BaseDbContext))]
[Migration("20260623120000_PuantajWorkTypeEnum")]
public partial class PuantajWorkTypeEnum : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "WorkType_New",
            table: "PuantajRecords",
            type: "int",
            nullable: false,
            defaultValue: 1);

        migrationBuilder.Sql(
            """
            UPDATE PuantajRecords
            SET WorkType_New = CASE
                WHEN WorkType IN (N'Gunduz', N'Gündüz', N'gündüz', N'gunduz', N'1') THEN 1
                WHEN WorkType IN (N'Gece', N'gece', N'2') THEN 2
                WHEN WorkType LIKE N'%Hafta%' OR WorkType = N'3' THEN 3
                WHEN WorkType LIKE N'%Tatil%' OR WorkType LIKE N'%tatil%' OR WorkType LIKE N'%Bayram%' OR WorkType = N'4' THEN 4
                WHEN WorkType IN (N'Izin', N'İzin', N'izin', N'5') THEN 5
                WHEN WorkType IN (N'Rapor', N'rapor', N'6') THEN 6
                ELSE 1
            END
            """);

        migrationBuilder.DropColumn(
            name: "WorkType",
            table: "PuantajRecords");

        migrationBuilder.RenameColumn(
            name: "WorkType_New",
            table: "PuantajRecords",
            newName: "WorkType");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "WorkType_Old",
            table: "PuantajRecords",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "Gündüz");

        migrationBuilder.Sql(
            """
            UPDATE PuantajRecords
            SET WorkType_Old = CASE WorkType
                WHEN 1 THEN N'Gündüz'
                WHEN 2 THEN N'Gece'
                WHEN 3 THEN N'Hafta Sonu'
                WHEN 4 THEN N'Resmi Tatil'
                WHEN 5 THEN N'İzin'
                WHEN 6 THEN N'Rapor'
                ELSE N'Gündüz'
            END
            """);

        migrationBuilder.DropColumn(
            name: "WorkType",
            table: "PuantajRecords");

        migrationBuilder.RenameColumn(
            name: "WorkType_Old",
            table: "PuantajRecords",
            newName: "WorkType");
    }
}
