using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnLTW.Migrations
{
    /// <inheritdoc />
    public partial class UpdateScheduleWithShifts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "DoctorWeeklySchedules");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "DoctorWeeklySchedules");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "DoctorWeeklySchedules",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Shift",
                table: "DoctorWeeklySchedules",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "DoctorWeeklySchedules");

            migrationBuilder.DropColumn(
                name: "Shift",
                table: "DoctorWeeklySchedules");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                table: "DoctorWeeklySchedules",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                table: "DoctorWeeklySchedules",
                type: "time",
                nullable: true);
        }
    }
}
