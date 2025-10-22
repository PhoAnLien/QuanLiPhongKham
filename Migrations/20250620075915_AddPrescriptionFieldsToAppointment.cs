using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnLTW.Migrations
{
    /// <inheritdoc />
    public partial class AddPrescriptionFieldsToAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Diagnosis",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DoctorNote",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prescription",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AppointmentDate", "CreatedAt", "Diagnosis", "DoctorNote", "Prescription" },
                values: new object[] { new DateTime(2025, 6, 21, 14, 59, 12, 810, DateTimeKind.Local).AddTicks(8631), new DateTime(2025, 6, 20, 7, 59, 12, 810, DateTimeKind.Utc).AddTicks(8673), null, null, null });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Diagnosis",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "DoctorNote",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Prescription",
                table: "Appointments");

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AppointmentDate", "CreatedAt" },
                values: new object[] { new DateTime(2025, 6, 21, 13, 52, 28, 723, DateTimeKind.Local).AddTicks(9467), new DateTime(2025, 6, 20, 6, 52, 28, 723, DateTimeKind.Utc).AddTicks(9515) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp", "VerifiedAt" },
                values: new object[] { "654afafc-ab85-4ba7-b6e7-c5e50b21af93", "8a8d5e08-67f1-41c0-b78f-ada43a120945", new DateTime(2025, 6, 20, 6, 52, 28, 723, DateTimeKind.Utc).AddTicks(9236) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp", "VerifiedAt" },
                values: new object[] { "42c97481-d211-4117-a838-1993e7386dc9", "00650955-8e6a-4ece-8adf-e1f1a2f42121", new DateTime(2025, 6, 20, 6, 52, 28, 723, DateTimeKind.Utc).AddTicks(9401) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp", "VerifiedAt" },
                values: new object[] { "cb1bbdc7-a178-4943-8ace-ebba65714154", "47ecd817-957f-4247-bd39-f1cbc893eb85", new DateTime(2025, 6, 20, 6, 52, 28, 723, DateTimeKind.Utc).AddTicks(9422) });
        }
    }
}
