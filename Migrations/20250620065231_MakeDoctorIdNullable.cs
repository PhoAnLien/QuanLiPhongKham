using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnLTW.Migrations
{
    /// <inheritdoc />
    public partial class MakeDoctorIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DoctorId",
                table: "Appointments",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DoctorId",
                table: "Appointments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AppointmentDate", "CreatedAt" },
                values: new object[] { new DateTime(2025, 6, 19, 13, 35, 15, 856, DateTimeKind.Local).AddTicks(9660), new DateTime(2025, 6, 18, 6, 35, 15, 856, DateTimeKind.Utc).AddTicks(9697) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp", "VerifiedAt" },
                values: new object[] { "dabb9d15-8abc-47b7-9fd8-9f087edac6db", "eb735596-b7c1-45e4-a76e-bb2fe0d566d1", new DateTime(2025, 6, 18, 6, 35, 15, 856, DateTimeKind.Utc).AddTicks(9514) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp", "VerifiedAt" },
                values: new object[] { "f8109dae-bab2-4ef7-9568-453718a6776f", "f7df408a-a842-4a76-b656-11e1f6ef9702", new DateTime(2025, 6, 18, 6, 35, 15, 856, DateTimeKind.Utc).AddTicks(9615) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp", "VerifiedAt" },
                values: new object[] { "86d0a317-061f-405e-9481-fb12c4d7918e", "7871165c-72cd-4c4b-ad4a-febdf3088c3b", new DateTime(2025, 6, 18, 6, 35, 15, 856, DateTimeKind.Utc).AddTicks(9631) });
        }
    }
}
