using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHakedisDeductionLines : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserOperationClaims",
                keyColumn: "Id",
                keyValue: new Guid("9cd499c8-f576-4d30-afb2-3a4d9ea538a1"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d32f9ff3-8d84-44c3-a4eb-4a3a4d5da9e2"));

            migrationBuilder.CreateTable(
                name: "HakedisDeductionLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HakedisPeriodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HakedisDeductionLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HakedisDeductionLines_HakedisPeriods_HakedisPeriodId",
                        column: x => x.HakedisPeriodId,
                        principalTable: "HakedisPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HakedisDeductionLines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AuthenticatorType", "CreatedDate", "DeletedDate", "Email", "PasswordHash", "PasswordSalt", "TenantId", "UpdatedDate" },
                values: new object[] { new Guid("5bba1952-5044-4c8c-89c3-59527756b2e0"), 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "narch@kodlama.io", new byte[] { 9, 119, 202, 118, 196, 27, 227, 184, 31, 43, 41, 147, 157, 196, 52, 183, 6, 22, 86, 218, 78, 175, 254, 94, 65, 125, 92, 87, 157, 0, 77, 150, 198, 35, 124, 100, 76, 187, 97, 222, 124, 197, 69, 102, 105, 17, 254, 230, 248, 35, 180, 21, 105, 253, 194, 162, 221, 178, 215, 114, 1, 33, 130, 161 }, new byte[] { 251, 174, 84, 79, 165, 71, 249, 144, 152, 200, 48, 229, 58, 4, 192, 26, 205, 77, 111, 21, 27, 132, 228, 29, 81, 203, 196, 45, 254, 34, 113, 158, 59, 216, 80, 231, 79, 78, 153, 252, 189, 40, 95, 160, 139, 66, 241, 76, 73, 25, 106, 155, 8, 254, 199, 145, 57, 58, 138, 197, 53, 4, 85, 102, 195, 38, 224, 33, 99, 52, 229, 238, 106, 165, 18, 73, 246, 3, 77, 119, 237, 44, 36, 182, 13, 152, 190, 130, 51, 135, 59, 74, 136, 128, 202, 106, 131, 177, 218, 187, 163, 239, 249, 144, 203, 91, 49, 22, 27, 28, 218, 149, 207, 71, 26, 100, 177, 244, 245, 49, 203, 242, 236, 166, 213, 19, 221, 253 }, null, null });

            migrationBuilder.InsertData(
                table: "UserOperationClaims",
                columns: new[] { "Id", "CreatedDate", "DeletedDate", "OperationClaimId", "UpdatedDate", "UserId" },
                values: new object[] { new Guid("a455f44d-e87c-46f8-8187-35df6f6ff130"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, null, new Guid("5bba1952-5044-4c8c-89c3-59527756b2e0") });

            migrationBuilder.CreateIndex(
                name: "IX_HakedisDeductionLines_HakedisPeriodId",
                table: "HakedisDeductionLines",
                column: "HakedisPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_HakedisDeductionLines_TenantId",
                table: "HakedisDeductionLines",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HakedisDeductionLines");

            migrationBuilder.DeleteData(
                table: "UserOperationClaims",
                keyColumn: "Id",
                keyValue: new Guid("a455f44d-e87c-46f8-8187-35df6f6ff130"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5bba1952-5044-4c8c-89c3-59527756b2e0"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AuthenticatorType", "CreatedDate", "DeletedDate", "Email", "PasswordHash", "PasswordSalt", "TenantId", "UpdatedDate" },
                values: new object[] { new Guid("d32f9ff3-8d84-44c3-a4eb-4a3a4d5da9e2"), 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "narch@kodlama.io", new byte[] { 136, 156, 37, 172, 115, 175, 160, 220, 188, 239, 29, 45, 169, 157, 2, 37, 211, 177, 201, 86, 148, 115, 253, 237, 83, 235, 6, 209, 167, 180, 65, 219, 94, 230, 11, 210, 120, 191, 1, 210, 81, 78, 222, 201, 39, 130, 106, 39, 8, 68, 200, 17, 84, 7, 148, 14, 61, 125, 220, 61, 24, 210, 194, 138 }, new byte[] { 94, 253, 205, 69, 71, 75, 8, 122, 30, 7, 145, 55, 138, 155, 36, 32, 184, 184, 28, 253, 119, 62, 186, 70, 93, 166, 15, 230, 200, 25, 53, 238, 183, 20, 2, 14, 138, 35, 205, 43, 147, 12, 5, 157, 64, 127, 154, 57, 238, 236, 50, 223, 178, 52, 149, 51, 70, 12, 199, 36, 20, 65, 37, 35, 38, 225, 253, 170, 189, 22, 103, 188, 142, 34, 203, 186, 20, 55, 169, 198, 91, 184, 163, 133, 149, 249, 9, 151, 148, 169, 184, 4, 183, 212, 58, 242, 129, 196, 183, 144, 12, 230, 33, 185, 34, 138, 100, 6, 93, 72, 177, 117, 149, 177, 254, 149, 148, 212, 77, 153, 3, 220, 114, 35, 196, 133, 200, 139 }, null, null });

            migrationBuilder.InsertData(
                table: "UserOperationClaims",
                columns: new[] { "Id", "CreatedDate", "DeletedDate", "OperationClaimId", "UpdatedDate", "UserId" },
                values: new object[] { new Guid("9cd499c8-f576-4d30-afb2-3a4d9ea538a1"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, null, new Guid("d32f9ff3-8d84-44c3-a4eb-4a3a4d5da9e2") });
        }
    }
}
