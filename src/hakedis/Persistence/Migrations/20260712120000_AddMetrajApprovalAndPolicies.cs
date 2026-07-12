using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Persistence.Contexts;

#nullable disable

namespace Persistence.Migrations;

[DbContext(typeof(BaseDbContext))]
[Migration("20260712120000_AddMetrajApprovalAndPolicies")]
public partial class AddMetrajApprovalAndPolicies : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            IF COL_LENGTH(N'dbo.MetrajResults', N'GrossQuantity') IS NULL
            BEGIN
                ALTER TABLE dbo.MetrajResults ADD GrossQuantity decimal(18,4) NOT NULL CONSTRAINT DF_MetrajResults_GrossQuantity DEFAULT 0;
                ALTER TABLE dbo.MetrajResults ADD SuggestedQuantity decimal(18,4) NULL;
                ALTER TABLE dbo.MetrajResults ADD ApprovalStatus int NOT NULL CONSTRAINT DF_MetrajResults_ApprovalStatus DEFAULT 1;
                ALTER TABLE dbo.MetrajResults ADD JudgmentDecision int NULL;
                ALTER TABLE dbo.MetrajResults ADD JudgmentReason nvarchar(2000) NULL;
                ALTER TABLE dbo.MetrajResults ADD PolicyRef nvarchar(50) NULL;
                ALTER TABLE dbo.MetrajResults ADD AiConfidence decimal(5,4) NULL;
                ALTER TABLE dbo.MetrajResults ADD IsLocked bit NOT NULL CONSTRAINT DF_MetrajResults_IsLocked DEFAULT 0;
                ALTER TABLE dbo.MetrajResults ADD ReviewedByUserId uniqueidentifier NULL;
                ALTER TABLE dbo.MetrajResults ADD ReviewedAt datetime2 NULL;

                -- UPDATE ayrı derlensin (SQL Server batch'te henüz var olmayan kolonu bağlayamaz)
                EXEC(N'UPDATE dbo.MetrajResults SET GrossQuantity = Quantity WHERE GrossQuantity = 0;');
            END

            IF OBJECT_ID(N'dbo.MetrajPolicies', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.MetrajPolicies (
                    Id uniqueidentifier NOT NULL,
                    TenantId uniqueidentifier NOT NULL,
                    Code nvarchar(50) NOT NULL,
                    Title nvarchar(200) NOT NULL,
                    Body nvarchar(max) NOT NULL,
                    Version int NOT NULL CONSTRAINT DF_MetrajPolicies_Version DEFAULT 1,
                    IsActive bit NOT NULL CONSTRAINT DF_MetrajPolicies_IsActive DEFAULT 1,
                    CreatedDate datetime2 NOT NULL,
                    UpdatedDate datetime2 NULL,
                    DeletedDate datetime2 NULL,
                    CONSTRAINT PK_MetrajPolicies PRIMARY KEY (Id),
                    CONSTRAINT FK_MetrajPolicies_Tenants_TenantId
                        FOREIGN KEY (TenantId) REFERENCES dbo.Tenants(Id)
                );
                CREATE UNIQUE INDEX IX_MetrajPolicies_TenantId_Code
                    ON dbo.MetrajPolicies(TenantId, Code)
                    WHERE DeletedDate IS NULL;
            END
            """
        );
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            IF OBJECT_ID(N'dbo.MetrajPolicies', N'U') IS NOT NULL
                DROP TABLE dbo.MetrajPolicies;

            IF COL_LENGTH(N'dbo.MetrajResults', N'GrossQuantity') IS NOT NULL
            BEGIN
                ALTER TABLE dbo.MetrajResults DROP CONSTRAINT DF_MetrajResults_GrossQuantity;
                ALTER TABLE dbo.MetrajResults DROP CONSTRAINT DF_MetrajResults_ApprovalStatus;
                ALTER TABLE dbo.MetrajResults DROP CONSTRAINT DF_MetrajResults_IsLocked;
                ALTER TABLE dbo.MetrajResults DROP COLUMN GrossQuantity;
                ALTER TABLE dbo.MetrajResults DROP COLUMN SuggestedQuantity;
                ALTER TABLE dbo.MetrajResults DROP COLUMN ApprovalStatus;
                ALTER TABLE dbo.MetrajResults DROP COLUMN JudgmentDecision;
                ALTER TABLE dbo.MetrajResults DROP COLUMN JudgmentReason;
                ALTER TABLE dbo.MetrajResults DROP COLUMN PolicyRef;
                ALTER TABLE dbo.MetrajResults DROP COLUMN AiConfidence;
                ALTER TABLE dbo.MetrajResults DROP COLUMN IsLocked;
                ALTER TABLE dbo.MetrajResults DROP COLUMN ReviewedByUserId;
                ALTER TABLE dbo.MetrajResults DROP COLUMN ReviewedAt;
            END
            """
        );
    }
}
