using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations;

/// <inheritdoc />
public partial class AddRole : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            IF NOT EXISTS (
                SELECT 1 FROM sys.columns
                WHERE object_id = OBJECT_ID(N'dbo.Users') AND name = N'FirmRole'
            )
            BEGIN
                ALTER TABLE dbo.Users ADD FirmRole int NULL;
            END;

            IF NOT EXISTS (
                SELECT 1 FROM sys.columns
                WHERE object_id = OBJECT_ID(N'dbo.Users') AND name = N'SecondaryFirmRole'
            )
            BEGIN
                ALTER TABLE dbo.Users ADD SecondaryFirmRole int NULL;
            END;
            """
        );

        migrationBuilder.Sql(
            """
            DELETE FROM dbo.UserOperationClaims
            WHERE Id IN (
                'a455f44d-e87c-46f8-8187-35df6f6ff130',
                'eac3afcd-86e6-4751-aa8d-608fce4444be'
            )
               OR UserId IN (
                '5bba1952-5044-4c8c-89c3-59527756b2e0',
                'dfc3b388-1c0e-4c92-bb77-759140f27e11'
            );

            DELETE FROM dbo.Users
            WHERE Id IN (
                '5bba1952-5044-4c8c-89c3-59527756b2e0',
                'dfc3b388-1c0e-4c92-bb77-759140f27e11'
            );
            """
        );

        migrationBuilder.InsertData(
            table: "Users",
            columns: new[]
            {
                "Id",
                "AuthenticatorType",
                "CreatedDate",
                "DeletedDate",
                "Email",
                "FirmRole",
                "PasswordHash",
                "PasswordSalt",
                "SecondaryFirmRole",
                "TenantId",
                "UpdatedDate",
            },
            values: new object[]
            {
                new Guid("dfc3b388-1c0e-4c92-bb77-759140f27e11"),
                0,
                new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                null,
                "narch@kodlama.io",
                null,
                new byte[]
                {
                    115, 118, 94, 85, 174, 33, 147, 175, 91, 96, 136, 203, 181, 140, 255, 52, 223, 195, 145, 99,
                    160, 50, 203, 182, 175, 190, 198, 209, 113, 204, 231, 213, 73, 26, 221, 116, 148, 150, 33, 111,
                    232, 89, 224, 43, 46, 157, 157, 170, 25, 234, 129, 139, 105, 45, 199, 183, 16, 195, 229, 142, 88,
                    232, 157, 234,
                },
                new byte[]
                {
                    148, 168, 22, 69, 85, 174, 185, 212, 81, 13, 189, 223, 227, 194, 46, 75, 91, 252, 81, 185, 190, 75,
                    194, 162, 223, 179, 255, 99, 236, 246, 26, 99, 49, 142, 119, 10, 27, 117, 74, 133, 124, 7, 150, 67,
                    95, 16, 14, 217, 96, 168, 177, 187, 73, 151, 103, 93, 175, 91, 226, 163, 36, 9, 136, 173, 185, 51,
                    46, 179, 130, 10, 26, 192, 156, 139, 153, 189, 151, 134, 8, 25, 217, 56, 111, 118, 91, 242, 7, 45,
                    112, 159, 14, 246, 95, 176, 62, 90, 44, 235, 192, 53, 50, 191, 39, 154, 246, 162, 236, 213, 224,
                    239, 123, 215, 183, 58, 15, 100, 166, 236, 197, 156, 47, 96, 41, 135, 177, 100, 215, 165,
                },
                null,
                null,
                null,
            }
        );

        migrationBuilder.InsertData(
            table: "UserOperationClaims",
            columns: new[] { "Id", "CreatedDate", "DeletedDate", "OperationClaimId", "UpdatedDate", "UserId" },
            values: new object[]
            {
                new Guid("eac3afcd-86e6-4751-aa8d-608fce4444be"),
                new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                null,
                1,
                null,
                new Guid("dfc3b388-1c0e-4c92-bb77-759140f27e11"),
            }
        );
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "UserOperationClaims",
            keyColumn: "Id",
            keyValue: new Guid("eac3afcd-86e6-4751-aa8d-608fce4444be")
        );

        migrationBuilder.DeleteData(
            table: "Users",
            keyColumn: "Id",
            keyValue: new Guid("dfc3b388-1c0e-4c92-bb77-759140f27e11")
        );

        migrationBuilder.DropColumn(name: "FirmRole", table: "Users");

        migrationBuilder.DropColumn(name: "SecondaryFirmRole", table: "Users");

        migrationBuilder.InsertData(
            table: "Users",
            columns: new[]
            {
                "Id",
                "AuthenticatorType",
                "CreatedDate",
                "DeletedDate",
                "Email",
                "PasswordHash",
                "PasswordSalt",
                "TenantId",
                "UpdatedDate",
            },
            values: new object[]
            {
                new Guid("5bba1952-5044-4c8c-89c3-59527756b2e0"),
                0,
                new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                null,
                "narch@kodlama.io",
                new byte[]
                {
                    9, 119, 202, 118, 196, 27, 227, 184, 31, 43, 41, 147, 157, 196, 52, 183, 6, 22, 86, 218, 78,
                    175, 254, 94, 65, 125, 92, 87, 157, 0, 77, 150, 198, 35, 124, 100, 76, 187, 97, 222, 124, 197,
                    69, 102, 105, 17, 254, 230, 248, 35, 180, 21, 105, 253, 194, 162, 221, 178, 215, 114, 1, 33,
                    130, 161,
                },
                new byte[]
                {
                    251, 174, 84, 79, 165, 71, 249, 144, 152, 200, 48, 229, 58, 4, 192, 26, 205, 77, 111, 21, 27,
                    132, 228, 29, 81, 203, 196, 45, 254, 34, 113, 158, 59, 216, 80, 231, 79, 78, 153, 252, 189, 40,
                    95, 160, 139, 66, 241, 76, 73, 25, 106, 155, 8, 254, 199, 145, 57, 58, 138, 197, 53, 4, 85, 102,
                    195, 38, 224, 33, 99, 52, 229, 238, 106, 165, 18, 73, 246, 3, 77, 119, 237, 44, 36, 182, 13, 152,
                    190, 130, 51, 135, 59, 74, 136, 128, 202, 106, 131, 177, 218, 187, 163, 239, 249, 144, 203, 91,
                    49, 22, 27, 28, 218, 149, 207, 71, 26, 100, 177, 244, 245, 49, 203, 242, 236, 166, 213, 19, 221,
                    253,
                },
                null,
                null,
            }
        );

        migrationBuilder.InsertData(
            table: "UserOperationClaims",
            columns: new[] { "Id", "CreatedDate", "DeletedDate", "OperationClaimId", "UpdatedDate", "UserId" },
            values: new object[]
            {
                new Guid("a455f44d-e87c-46f8-8187-35df6f6ff130"),
                new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                null,
                1,
                null,
                new Guid("5bba1952-5044-4c8c-89c3-59527756b2e0"),
            }
        );
    }
}
