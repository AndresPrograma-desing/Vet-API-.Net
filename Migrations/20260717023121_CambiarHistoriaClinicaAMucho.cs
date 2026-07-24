using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vet_api_Net.Migrations
{
    /// <inheritdoc />
    public partial class CambiarHistoriaClinicaAMucho : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_hc_mascota",
                table: "historias_clinicas");

            migrationBuilder.CreateIndex(
                name: "idx_hc_mascota",
                table: "historias_clinicas",
                column: "mascota_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_hc_mascota",
                table: "historias_clinicas");

            migrationBuilder.CreateIndex(
                name: "idx_hc_mascota",
                table: "historias_clinicas",
                column: "mascota_id",
                unique: true);
        }
    }
}
