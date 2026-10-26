using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace vet_api_Net.Migrations
{
    /// <inheritdoc />
    public partial class AddServicioCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "servicio_id",
                table: "detalles_factura",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "servicios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    precio = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    creado = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    actualizado = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_servicios", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_detalles_factura_servicio",
                table: "detalles_factura",
                column: "servicio_id");

            migrationBuilder.CreateIndex(
                name: "IX_servicios_nombre",
                table: "servicios",
                column: "nombre",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "detalles_factura_ibfk_5",
                table: "detalles_factura",
                column: "servicio_id",
                principalTable: "servicios",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "detalles_factura_ibfk_5",
                table: "detalles_factura");

            migrationBuilder.DropTable(
                name: "servicios");

            migrationBuilder.DropIndex(
                name: "idx_detalles_factura_servicio",
                table: "detalles_factura");

            migrationBuilder.DropColumn(
                name: "servicio_id",
                table: "detalles_factura");
        }
    }
}
