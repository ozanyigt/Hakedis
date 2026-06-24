using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Persistence.Contexts;

#nullable disable

namespace Persistence.Migrations;

[DbContext(typeof(BaseDbContext))]
[Migration("20260625120000_AddProjectMetrajLayerMappings")]
public partial class AddProjectMetrajLayerMappings : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            IF OBJECT_ID(N'dbo.ProjectMetrajLayerMappings', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.ProjectMetrajLayerMappings (
                    Id uniqueidentifier NOT NULL,
                    TenantId uniqueidentifier NOT NULL,
                    ProjectId uniqueidentifier NOT NULL,
                    KalemType int NOT NULL,
                    LayerNames nvarchar(max) NOT NULL CONSTRAINT DF_ProjectMetrajLayerMappings_LayerNames DEFAULT '',
                    CreatedDate datetime2 NOT NULL,
                    UpdatedDate datetime2 NULL,
                    DeletedDate datetime2 NULL,
                    CONSTRAINT PK_ProjectMetrajLayerMappings PRIMARY KEY (Id),
                    CONSTRAINT FK_ProjectMetrajLayerMappings_Projects_ProjectId
                        FOREIGN KEY (ProjectId) REFERENCES dbo.Projects(Id) ON DELETE CASCADE,
                    CONSTRAINT FK_ProjectMetrajLayerMappings_Tenants_TenantId
                        FOREIGN KEY (TenantId) REFERENCES dbo.Tenants(Id)
                );

                CREATE UNIQUE INDEX IX_ProjectMetrajLayerMappings_ProjectId_KalemType
                    ON dbo.ProjectMetrajLayerMappings(ProjectId, KalemType)
                    WHERE DeletedDate IS NULL;
            END
            """
        );
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "ProjectMetrajLayerMappings");
    }
}
