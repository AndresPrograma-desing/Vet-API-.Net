using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vet_api_Net.Migrations
{
    /// <inheritdoc />
    public partial class DecoupleVaccineFromProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "detalles_factura_ibfk_2",
                table: "detalles_factura");

            migrationBuilder.DropForeignKey(
                name: "vaccines_ibfk_1",
                table: "vaccines");

            migrationBuilder.DropIndex(
                name: "idx_vaccine_producto",
                table: "vaccines");

            migrationBuilder.DropColumn(
                name: "producto_id",
                table: "vaccines");

            migrationBuilder.AlterColumn<int>(
                name: "producto_id",
                table: "detalles_factura",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "vaccine_id",
                table: "detalles_factura",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "idx_detalles_factura_vaccine",
                table: "detalles_factura",
                column: "vaccine_id");

            migrationBuilder.AddForeignKey(
                name: "detalles_factura_ibfk_2",
                table: "detalles_factura",
                column: "producto_id",
                principalTable: "productos",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "detalles_factura_ibfk_4",
                table: "detalles_factura",
                column: "vaccine_id",
                principalTable: "vaccines",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "detalles_factura_ibfk_2",
                table: "detalles_factura");

            migrationBuilder.DropForeignKey(
                name: "detalles_factura_ibfk_4",
                table: "detalles_factura");

            migrationBuilder.DropIndex(
                name: "idx_detalles_factura_vaccine",
                table: "detalles_factura");

            migrationBuilder.DropColumn(
                name: "vaccine_id",
                table: "detalles_factura");

            migrationBuilder.AddColumn<int>(
                name: "producto_id",
                table: "vaccines",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "producto_id",
                table: "detalles_factura",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "idx_vaccine_producto",
                table: "vaccines",
                column: "producto_id");

            migrationBuilder.AddForeignKey(
                name: "detalles_factura_ibfk_2",
                table: "detalles_factura",
                column: "producto_id",
                principalTable: "productos",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "vaccines_ibfk_1",
                table: "vaccines",
                column: "producto_id",
                principalTable: "productos",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
