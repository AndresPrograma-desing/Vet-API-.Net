using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vet_api_Net.Migrations
{
    /// <inheritdoc />
    public partial class WSMessageAPIData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WSMessageAPIData",
                columns: table => new
                {
                    ClientId = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8mb4_spanish2_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ApiKey = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_spanish2_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Update = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WSMessageAPIData", x => x.ClientId);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_spanish2_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WSMessageAPIData");
        }
    }
}
