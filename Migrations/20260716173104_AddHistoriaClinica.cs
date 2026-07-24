using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace vet_api_Net.Migrations
{
    /// <inheritdoc />
    public partial class AddHistoriaClinica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "historias_clinicas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mascota_id = table.Column<int>(type: "integer", nullable: false),
                    resumen_ia = table.Column<string>(type: "text", nullable: true),
                    alertas_riesgo_ia = table.Column<string>(type: "text", nullable: true),
                    sugerencias_ia = table.Column<string>(type: "text", nullable: true),
                    notas_veterinario = table.Column<string>(type: "text", nullable: true),
                    ultimo_analisis_ia = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    creado = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    actualizado = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historias_clinicas", x => x.id);
                    table.ForeignKey(
                        name: "fk_historia_clinica_mascota",
                        column: x => x.mascota_id,
                        principalTable: "mascotas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_hc_mascota",
                table: "historias_clinicas",
                column: "mascota_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "historias_clinicas");
        }
    }
}
