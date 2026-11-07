using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vet_api_Net.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkingHoursToSystemConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeOnly>(
                name: "work_end_time",
                table: "system_configs",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(20, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "work_start_time",
                table: "system_configs",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(8, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "work_end_time",
                table: "system_configs");

            migrationBuilder.DropColumn(
                name: "work_start_time",
                table: "system_configs");
        }
    }
}
