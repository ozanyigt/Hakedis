using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations;

public partial class AddDailySiteReports : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "DailySiteReports",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ReportDate = table.Column<DateTime>(type: "date", nullable: false),
                Weather = table.Column<int>(type: "int", nullable: false),
                MinTemperatureCelsius = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                MaxTemperatureCelsius = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                WorkSummary = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                WorkforceNotes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                EquipmentNotes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                MaterialNotes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                BlockersNotes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                RejectionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ApprovedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                Status = table.Column<int>(type: "int", nullable: false),
                CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DailySiteReports", x => x.Id);
                table.ForeignKey("FK_DailySiteReports_Projects_ProjectId", x => x.ProjectId, "Projects", "Id");
                table.ForeignKey("FK_DailySiteReports_Sites_SiteId", x => x.SiteId, "Sites", "Id");
                table.ForeignKey("FK_DailySiteReports_Tenants_TenantId", x => x.TenantId, "Tenants", "Id");
                table.ForeignKey("FK_DailySiteReports_Users_ApprovedByUserId", x => x.ApprovedByUserId, "Users", "Id");
                table.ForeignKey("FK_DailySiteReports_Users_CreatedByUserId", x => x.CreatedByUserId, "Users", "Id");
            });

        migrationBuilder.CreateTable(
            name: "DailySiteReportPhotos",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                DailySiteReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Url = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                SortOrder = table.Column<int>(type: "int", nullable: false),
                CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DailySiteReportPhotos", x => x.Id);
                table.ForeignKey(
                    "FK_DailySiteReportPhotos_DailySiteReports_DailySiteReportId",
                    x => x.DailySiteReportId,
                    "DailySiteReports",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex("IX_DailySiteReportPhotos_DailySiteReportId", "DailySiteReportPhotos", "DailySiteReportId");
        migrationBuilder.CreateIndex("IX_DailySiteReports_ApprovedByUserId", "DailySiteReports", "ApprovedByUserId");
        migrationBuilder.CreateIndex("IX_DailySiteReports_CreatedByUserId", "DailySiteReports", "CreatedByUserId");
        migrationBuilder.CreateIndex("IX_DailySiteReports_ProjectId", "DailySiteReports", "ProjectId");
        migrationBuilder.CreateIndex("IX_DailySiteReports_SiteId", "DailySiteReports", "SiteId");
        migrationBuilder.CreateIndex("IX_DailySiteReports_TenantId", "DailySiteReports", "TenantId");
        migrationBuilder.CreateIndex(
            name: "UX_DailySiteReports_Tenant_Site_Date_Active",
            table: "DailySiteReports",
            columns: new[] { "TenantId", "SiteId", "ReportDate" },
            unique: true,
            filter: "[DeletedDate] IS NULL");

        migrationBuilder.InsertData(
            table: "OperationClaims",
            columns: new[] { "Id", "CreatedDate", "DeletedDate", "Name", "UpdatedDate" },
            values: new object[,]
            {
                { 102, DateTime.MinValue, null, "DailySiteReports.Admin", null },
                { 103, DateTime.MinValue, null, "DailySiteReports.Read", null },
                { 104, DateTime.MinValue, null, "DailySiteReports.Write", null },
                { 105, DateTime.MinValue, null, "DailySiteReports.Create", null },
                { 106, DateTime.MinValue, null, "DailySiteReports.Update", null },
                { 107, DateTime.MinValue, null, "DailySiteReports.Delete", null }
            });

        migrationBuilder.Sql(
            """
            INSERT INTO UserOperationClaims
                (Id, UserId, OperationClaimId, CreatedDate, UpdatedDate, DeletedDate)
            SELECT NEWID(), users.Id, claims.Id, SYSUTCDATETIME(), NULL, NULL
            FROM Users AS users
            INNER JOIN
            (
                VALUES
                    (1, N'DailySiteReports.Admin'),
                    (2, N'DailySiteReports.Admin'),
                    (3, N'DailySiteReports.Read'),
                    (3, N'DailySiteReports.Write'),
                    (3, N'DailySiteReports.Create'),
                    (3, N'DailySiteReports.Update'),
                    (4, N'DailySiteReports.Read'),
                    (6, N'DailySiteReports.Read')
            ) AS roleClaims(RoleValue, ClaimName)
                ON roleClaims.RoleValue = users.FirmRole
                OR roleClaims.RoleValue = users.SecondaryFirmRole
            INNER JOIN OperationClaims AS claims
                ON claims.Name = roleClaims.ClaimName
                AND claims.DeletedDate IS NULL
            WHERE users.DeletedDate IS NULL
              AND NOT EXISTS
              (
                  SELECT 1
                  FROM UserOperationClaims AS existing
                  WHERE existing.UserId = users.Id
                    AND existing.OperationClaimId = claims.Id
                    AND existing.DeletedDate IS NULL
              );
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "DailySiteReportPhotos");
        migrationBuilder.DropTable(name: "DailySiteReports");
        migrationBuilder.Sql(
            """
            DELETE FROM UserOperationClaims
            WHERE OperationClaimId IN (102, 103, 104, 105, 106, 107);
            """);
        migrationBuilder.DeleteData(table: "OperationClaims", keyColumn: "Id", keyValues: new object[] { 102, 103, 104, 105, 106, 107 });
    }
}
