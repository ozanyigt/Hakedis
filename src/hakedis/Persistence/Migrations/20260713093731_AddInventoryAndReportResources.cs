using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814

namespace Persistence.Migrations
{
    public partial class AddInventoryAndReportResources : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PuantajRecords_TenantId",
                table: "PuantajRecords");

            migrationBuilder.CreateTable(
                name: "DailySiteReportWorkforceSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DailySiteReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourcePuantajRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WorkerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Trade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    WorkType = table.Column<int>(type: "int", nullable: false),
                    DayCount = table.Column<decimal>(
                        type: "decimal(18,4)",
                        precision: 18,
                        scale: 4,
                        nullable: false),
                    OvertimeHours = table.Column<decimal>(
                        type: "decimal(18,4)",
                        precision: 18,
                        scale: 4,
                        nullable: false),
                    PuantajStatusAtCapture = table.Column<int>(type: "int", nullable: false),
                    CaptureBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CapturedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailySiteReportWorkforceSnapshots", x => x.Id);

                    table.ForeignKey(
                        name: "FK_DailySiteReportWorkforceSnapshots_DailySiteReports_DailySiteReportId",
                        column: x => x.DailySiteReportId,
                        principalTable: "DailySiteReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: false),
                    Name = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: false),
                    Unit = table.Column<string>(
                        type: "nvarchar(30)",
                        maxLength: 30,
                        nullable: false),
                    Description = table.Column<string>(
                        type: "nvarchar(1000)",
                        maxLength: 1000,
                        nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.Id);

                    table.ForeignKey(
                        name: "FK_Materials_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DailySiteReportMaterialLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DailySiteReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialCode = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: false),
                    MaterialName = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: false),
                    Unit = table.Column<string>(
                        type: "nvarchar(30)",
                        maxLength: 30,
                        nullable: false),
                    Quantity = table.Column<decimal>(
                        type: "decimal(18,4)",
                        precision: 18,
                        scale: 4,
                        nullable: false),
                    Notes = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true),
                    PostedUnitCost = table.Column<decimal>(
                        type: "decimal(18,6)",
                        precision: 18,
                        scale: 6,
                        nullable: true),
                    PostedTotalCost = table.Column<decimal>(
                        type: "decimal(18,6)",
                        precision: 18,
                        scale: 6,
                        nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailySiteReportMaterialLines", x => x.Id);

                    table.ForeignKey(
                        name: "FK_DailySiteReportMaterialLines_DailySiteReports_DailySiteReportId",
                        column: x => x.DailySiteReportId,
                        principalTable: "DailySiteReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);

                    table.ForeignKey(
                        name: "FK_DailySiteReportMaterialLines_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SiteStockBalances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(
                        type: "decimal(18,4)",
                        precision: 18,
                        scale: 4,
                        nullable: false),
                    AverageUnitCost = table.Column<decimal>(
                        type: "decimal(18,6)",
                        precision: 18,
                        scale: 6,
                        nullable: false),
                    RowVersion = table.Column<byte[]>(
                        type: "rowversion",
                        rowVersion: true,
                        nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteStockBalances", x => x.Id);

                    table.ForeignKey(
                        name: "FK_SiteStockBalances_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id");

                    table.ForeignKey(
                        name: "FK_SiteStockBalances_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id");

                    table.ForeignKey(
                        name: "FK_SiteStockBalances_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StockTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MovementType = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(
                        type: "decimal(18,4)",
                        precision: 18,
                        scale: 4,
                        nullable: false),
                    UnitCost = table.Column<decimal>(
                        type: "decimal(18,6)",
                        precision: 18,
                        scale: 6,
                        nullable: false),
                    TotalCost = table.Column<decimal>(
                        type: "decimal(18,6)",
                        precision: 18,
                        scale: 6,
                        nullable: false),
                    BalanceQuantityAfter = table.Column<decimal>(
                        type: "decimal(18,4)",
                        precision: 18,
                        scale: 4,
                        nullable: false),
                    AverageUnitCostAfter = table.Column<decimal>(
                        type: "decimal(18,6)",
                        precision: 18,
                        scale: 6,
                        nullable: false),
                    ReferenceType = table.Column<int>(type: "int", nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TransferId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IdempotencyKey = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: false),
                    Reference = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: true),
                    Notes = table.Column<string>(
                        type: "nvarchar(1000)",
                        maxLength: 1000,
                        nullable: true),
                    PostedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PostedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTransactions", x => x.Id);

                    table.ForeignKey(
                        name: "FK_StockTransactions_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id");

                    table.ForeignKey(
                        name: "FK_StockTransactions_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id");

                    table.ForeignKey(
                        name: "FK_StockTransactions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "OperationClaims",
                columns: new[]
                {
                    "Id",
                    "CreatedDate",
                    "DeletedDate",
                    "Name",
                    "UpdatedDate"
                },
                values: new object[,]
                {
                    {
                        108,
                        DateTime.MinValue,
                        null,
                        "Inventory.Admin",
                        null
                    },
                    {
                        109,
                        DateTime.MinValue,
                        null,
                        "Inventory.Read",
                        null
                    },
                    {
                        110,
                        DateTime.MinValue,
                        null,
                        "Inventory.Write",
                        null
                    },
                    {
                        111,
                        DateTime.MinValue,
                        null,
                        "Inventory.Create",
                        null
                    },
                    {
                        112,
                        DateTime.MinValue,
                        null,
                        "Inventory.Update",
                        null
                    },
                    {
                        113,
                        DateTime.MinValue,
                        null,
                        "Inventory.Delete",
                        null
                    }
                });

            migrationBuilder.Sql(
                """
                INSERT INTO UserOperationClaims
                    (Id, UserId, OperationClaimId, CreatedDate, UpdatedDate, DeletedDate)
                SELECT
                    NEWID(),
                    users.Id,
                    claims.Id,
                    SYSUTCDATETIME(),
                    NULL,
                    NULL
                FROM Users AS users
                INNER JOIN
                (
                    VALUES
                        (1, N'Inventory.Admin'),
                        (2, N'Inventory.Admin'),
                        (3, N'Inventory.Read'),
                        (5, N'Inventory.Read'),
                        (6, N'Inventory.Read')
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

            migrationBuilder.CreateIndex(
                name: "IX_PuantajRecords_TenantId_ProjectId_SiteId_WorkerId_WorkDate",
                table: "PuantajRecords",
                columns: new[]
                {
                    "TenantId",
                    "ProjectId",
                    "SiteId",
                    "WorkerId",
                    "WorkDate"
                },
                unique: true,
                filter: "[WorkerId] IS NOT NULL AND [DeletedDate] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DailySiteReportMaterialLines_DailySiteReportId_MaterialId",
                table: "DailySiteReportMaterialLines",
                columns: new[]
                {
                    "DailySiteReportId",
                    "MaterialId"
                },
                unique: true,
                filter: "[DeletedDate] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DailySiteReportMaterialLines_MaterialId",
                table: "DailySiteReportMaterialLines",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_DailySiteReportWorkforceSnapshots_DailySiteReportId_CaptureBatchId",
                table: "DailySiteReportWorkforceSnapshots",
                columns: new[]
                {
                    "DailySiteReportId",
                    "CaptureBatchId"
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailySiteReportWorkforceSnapshots_DailySiteReportId_SourcePuantajRecordId",
                table: "DailySiteReportWorkforceSnapshots",
                columns: new[]
                {
                    "DailySiteReportId",
                    "SourcePuantajRecordId"
                },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Materials_TenantId_Code",
                table: "Materials",
                columns: new[]
                {
                    "TenantId",
                    "Code"
                },
                unique: true,
                filter: "[DeletedDate] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SiteStockBalances_MaterialId",
                table: "SiteStockBalances",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_SiteStockBalances_SiteId",
                table: "SiteStockBalances",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_SiteStockBalances_TenantId_SiteId_MaterialId",
                table: "SiteStockBalances",
                columns: new[]
                {
                    "TenantId",
                    "SiteId",
                    "MaterialId"
                },
                unique: true,
                filter: "[DeletedDate] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_MaterialId",
                table: "StockTransactions",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_SiteId",
                table: "StockTransactions",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_TenantId_IdempotencyKey",
                table: "StockTransactions",
                columns: new[]
                {
                    "TenantId",
                    "IdempotencyKey"
                },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_TenantId_SiteId_MaterialId_OccurredAt",
                table: "StockTransactions",
                columns: new[]
                {
                    "TenantId",
                    "SiteId",
                    "MaterialId",
                    "OccurredAt"
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DELETE FROM UserOperationClaims
                WHERE OperationClaimId IN (108, 109, 110, 111, 112, 113);
                """);

            migrationBuilder.DropTable(
                name: "DailySiteReportMaterialLines");

            migrationBuilder.DropTable(
                name: "DailySiteReportWorkforceSnapshots");

            migrationBuilder.DropTable(
                name: "SiteStockBalances");

            migrationBuilder.DropTable(
                name: "StockTransactions");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropIndex(
                name: "IX_PuantajRecords_TenantId_ProjectId_SiteId_WorkerId_WorkDate",
                table: "PuantajRecords");

            migrationBuilder.DeleteData(
                table: "OperationClaims",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "OperationClaims",
                keyColumn: "Id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "OperationClaims",
                keyColumn: "Id",
                keyValue: 110);

            migrationBuilder.DeleteData(
                table: "OperationClaims",
                keyColumn: "Id",
                keyValue: 111);

            migrationBuilder.DeleteData(
                table: "OperationClaims",
                keyColumn: "Id",
                keyValue: 112);

            migrationBuilder.DeleteData(
                table: "OperationClaims",
                keyColumn: "Id",
                keyValue: 113);

            migrationBuilder.CreateIndex(
                name: "IX_PuantajRecords_TenantId",
                table: "PuantajRecords",
                column: "TenantId");
        }
    }
}