using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace vet_api_Net.Migrations
{
    /// <inheritdoc />
    public partial class AgregarIaConocimiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ia_conocimientos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    reglas_respuesta = table.Column<string>(type: "text", nullable: false),
                    base_conocimiento = table.Column<string>(type: "text", nullable: false),
                    categoria = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    creado = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    actualizado = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ia_conocimientos", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ia_conocimientos");
        }
    }
}
