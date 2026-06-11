using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vet_api_Net.Migrations
{
    /// <inheritdoc />
    public partial class AddReporteTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reportes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Titulo = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_spanish2_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Categoria = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_spanish2_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Filtro = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_spanish2_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Datos = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_spanish2_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GeneradoPor = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_spanish2_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reportes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_spanish2_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reportes");
        }
    }
}
