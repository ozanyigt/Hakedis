using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Persistence.Contexts;

#nullable disable

namespace Persistence.Migrations;

[DbContext(typeof(BaseDbContext))]
[Migration("20260626120000_AddDemoRequests")]
public partial class AddDemoRequests : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            IF OBJECT_ID(N'dbo.DemoRequests', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.DemoRequests (
                    Id uniqueidentifier NOT NULL,
                    CompanyName nvarchar(200) NOT NULL,
                    ContactName nvarchar(200) NOT NULL,
                    Email nvarchar(256) NOT NULL,
                    Phone nvarchar(50) NOT NULL,
                    Interest nvarchar(100) NOT NULL,
                    Message nvarchar(2000) NULL,
                    Status int NOT NULL CONSTRAINT DF_DemoRequests_Status DEFAULT 1,
                    CreatedDate datetime2 NOT NULL,
                    UpdatedDate datetime2 NULL,
                    DeletedDate datetime2 NULL,
                    CONSTRAINT PK_DemoRequests PRIMARY KEY (Id)
                );
                CREATE INDEX IX_DemoRequests_Status_CreatedDate ON dbo.DemoRequests(Status, CreatedDate DESC);
            END
            """
        );
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "DemoRequests");
    }
}
