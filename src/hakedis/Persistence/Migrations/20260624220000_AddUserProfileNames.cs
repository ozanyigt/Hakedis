using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Persistence.Contexts;

#nullable disable

namespace Persistence.Migrations;

[DbContext(typeof(BaseDbContext))]
[Migration("20260624220000_AddUserProfileNames")]
public partial class AddUserProfileNames : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            IF NOT EXISTS (
                SELECT 1 FROM sys.columns
                WHERE object_id = OBJECT_ID(N'dbo.Users') AND name = N'FirstName'
            )
            BEGIN
                ALTER TABLE dbo.Users ADD FirstName nvarchar(100) NOT NULL CONSTRAINT DF_Users_FirstName DEFAULT '';
            END;

            IF NOT EXISTS (
                SELECT 1 FROM sys.columns
                WHERE object_id = OBJECT_ID(N'dbo.Users') AND name = N'LastName'
            )
            BEGIN
                ALTER TABLE dbo.Users ADD LastName nvarchar(100) NOT NULL CONSTRAINT DF_Users_LastName DEFAULT '';
            END;
            """
        );
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "FirstName", table: "Users");
        migrationBuilder.DropColumn(name: "LastName", table: "Users");
    }
}
