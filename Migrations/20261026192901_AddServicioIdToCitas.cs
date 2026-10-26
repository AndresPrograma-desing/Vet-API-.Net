using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vet_api_Net.Migrations
{
    /// <inheritdoc />
    public partial class AddServicioIdToCitas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "servicio_id",
                table: "citas",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "idx_citas_servicio",
                table: "citas",
                column: "servicio_id");

            migrationBuilder.AddForeignKey(
                name: "citas_ibfk_servicio",
                table: "citas",
                column: "servicio_id",
                principalTable: "servicios",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "citas_ibfk_servicio",
                table: "citas");

            migrationBuilder.DropIndex(
                name: "idx_citas_servicio",
                table: "citas");

            migrationBuilder.DropColumn(
                name: "servicio_id",
                table: "citas");
        }
    }
}
