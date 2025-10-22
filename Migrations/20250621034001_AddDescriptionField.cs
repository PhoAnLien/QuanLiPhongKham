using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnLTW.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "VerifiedBy",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Appointments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ApplicationUserId", "AppointmentDate", "CreatedAt" },
                values: new object[] { null, new DateTime(2025, 6, 22, 10, 39, 58, 422, DateTimeKind.Local).AddTicks(7443), new DateTime(2025, 6, 21, 3, 39, 58, 422, DateTimeKind.Utc).AddTicks(7501) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "Description", "SecurityStamp", "VerifiedAt" },
                values: new object[] { "aff6507e-5778-4c47-9b41-3acce90e9a72", null, "99ee3546-a4f3-41b0-83d8-42e7292f72ba", new DateTime(2025, 6, 21, 3, 39, 58, 422, DateTimeKind.Utc).AddTicks(7130) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "ConcurrencyStamp", "Description", "SecurityStamp", "VerifiedAt" },
                values: new object[] { "a5d1a838-f100-4232-bede-7182c874840a", null, "6f6fb5ab-8fe5-4828-a4d0-1d9b2090c242", new DateTime(2025, 6, 21, 3, 39, 58, 422, DateTimeKind.Utc).AddTicks(7369) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3",
                columns: new[] { "ConcurrencyStamp", "Description", "SecurityStamp", "VerifiedAt" },
                values: new object[] { "a38b0674-7d22-49ab-80bb-faf3313c5b19", null, "25092623-2173-44af-ab7b-b24e30da8cd0", new DateTime(2025, 6, 21, 3, 39, 58, 422, DateTimeKind.Utc).AddTicks(7389) });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ApplicationUserId",
                table: "Appointments",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AspNetUsers_ApplicationUserId",
                table: "Appointments",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AspNetUsers_ApplicationUserId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_ApplicationUserId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Appointments");

            migrationBuilder.AlterColumn<string>(
                name: "VerifiedBy",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AppointmentDate", "CreatedAt" },
                values: new object[] { new DateTime(2025, 6, 21, 14, 59, 12, 810, DateTimeKind.Local).AddTicks(8631), new DateTime(2025, 6, 20, 7, 59, 12, 810, DateTimeKind.Utc).AddTicks(8673) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp", "VerifiedAt" },
                values: new object[] { "ef9c4ec6-ecc5-4450-b46b-d5ea519a7a28", "e720b30d-140f-4e80-821c-8361682766be", new DateTime(2025, 6, 20, 7, 59, 12, 810, DateTimeKind.Utc).AddTicks(8442) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp", "VerifiedAt" },
                values: new object[] { "545fe71b-c05d-4ed2-8bd2-b5156e5292e8", "49cc0ae6-27e2-4697-9961-115f23b73219", new DateTime(2025, 6, 20, 7, 59, 12, 810, DateTimeKind.Utc).AddTicks(8574) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp", "VerifiedAt" },
                values: new object[] { "989fbe19-f63c-4cdb-91d4-6753d9c7361c", "481d55e4-6ea9-4eda-a094-b6d587de1479", new DateTime(2025, 6, 20, 7, 59, 12, 810, DateTimeKind.Utc).AddTicks(8593) });
        }
    }
}
