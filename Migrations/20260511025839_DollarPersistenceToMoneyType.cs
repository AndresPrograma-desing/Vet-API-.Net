using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vet_api_Net.Migrations
{
    /// <inheritdoc />
    public partial class DollarPersistenceToMoneyType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Titulo",
                table: "Reportes",
                type: "longtext",
                nullable: true,
                collation: "utf8mb4_spanish2_ci",
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_spanish2_ci");

            migrationBuilder.AlterColumn<string>(
                name: "GeneradoPor",
                table: "Reportes",
                type: "longtext",
                nullable: true,
                collation: "utf8mb4_spanish2_ci",
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_spanish2_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Filtro",
                table: "Reportes",
                type: "longtext",
                nullable: true,
                collation: "utf8mb4_spanish2_ci",
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_spanish2_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Datos",
                table: "Reportes",
                type: "longtext",
                nullable: true,
                collation: "utf8mb4_spanish2_ci",
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_spanish2_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Categoria",
                table: "Reportes",
                type: "longtext",
                nullable: true,
                collation: "utf8mb4_spanish2_ci",
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_spanish2_ci");

            migrationBuilder.AddColumn<string>(
                name: "DollarPersistence",
                table: "MoneyTypes",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_spanish2_ci")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DollarPersistence",
                table: "MoneyTypes");

            migrationBuilder.UpdateData(
                table: "Reportes",
                keyColumn: "Titulo",
                keyValue: null,
                column: "Titulo",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Titulo",
                table: "Reportes",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_spanish2_ci",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_spanish2_ci");

            migrationBuilder.UpdateData(
                table: "Reportes",
                keyColumn: "GeneradoPor",
                keyValue: null,
                column: "GeneradoPor",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "GeneradoPor",
                table: "Reportes",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_spanish2_ci",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_spanish2_ci");

            migrationBuilder.UpdateData(
                table: "Reportes",
                keyColumn: "Filtro",
                keyValue: null,
                column: "Filtro",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Filtro",
                table: "Reportes",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_spanish2_ci",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_spanish2_ci");

            migrationBuilder.UpdateData(
                table: "Reportes",
                keyColumn: "Datos",
                keyValue: null,
                column: "Datos",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Datos",
                table: "Reportes",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_spanish2_ci",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_spanish2_ci");

            migrationBuilder.UpdateData(
                table: "Reportes",
                keyColumn: "Categoria",
                keyValue: null,
                column: "Categoria",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Categoria",
                table: "Reportes",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_spanish2_ci",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_spanish2_ci");
        }
    }
}
