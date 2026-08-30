using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace vet_api_Net.Migrations
{
    /// <inheritdoc />
    public partial class AddEspecieCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Crear la tabla del catálogo compartido de especies.
            migrationBuilder.CreateTable(
                name: "especies",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_especies", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_especies_nombre",
                table: "especies",
                column: "nombre",
                unique: true);

            // 2. Sembrar las dos especies canónicas.
            migrationBuilder.Sql("INSERT INTO especies (nombre) VALUES ('canino'), ('felino');");

            // 3. Agregar las columnas FK como nullable, para poder backfillear antes de forzar NOT NULL.
            migrationBuilder.AddColumn<int>(
                name: "especie_id",
                table: "vaccines",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "especie_id",
                table: "mascotas",
                type: "integer",
                nullable: true);

            // 4. Backfill mapeando ambos vocabularios históricos (perro/gato Y canino/felino) a las dos filas canónicas.
            migrationBuilder.Sql(@"
                UPDATE vaccines SET especie_id = (SELECT id FROM especies WHERE nombre = 'canino') WHERE lower(species) = 'canino';
                UPDATE vaccines SET especie_id = (SELECT id FROM especies WHERE nombre = 'felino') WHERE lower(species) = 'felino';
                UPDATE mascotas SET especie_id = (SELECT id FROM especies WHERE nombre = 'canino') WHERE lower(especie) IN ('perro', 'canino');
                UPDATE mascotas SET especie_id = (SELECT id FROM especies WHERE nombre = 'felino') WHERE lower(especie) IN ('gato', 'felino');
            ");

            // 4b. Fallback general: cualquier valor fuera de esas 4 palabras se registra como especie nueva,
            // para que la migración nunca falle por datos de producción inesperados.
            migrationBuilder.Sql(@"
                INSERT INTO especies (nombre)
                SELECT DISTINCT lower(trim(especie)) FROM mascotas
                WHERE especie_id IS NULL
                ON CONFLICT (nombre) DO NOTHING;

                INSERT INTO especies (nombre)
                SELECT DISTINCT lower(trim(species)) FROM vaccines
                WHERE especie_id IS NULL
                ON CONFLICT (nombre) DO NOTHING;

                UPDATE mascotas m SET especie_id = e.id
                FROM especies e WHERE m.especie_id IS NULL AND lower(trim(m.especie)) = e.nombre;

                UPDATE vaccines v SET especie_id = e.id
                FROM especies e WHERE v.especie_id IS NULL AND lower(trim(v.species)) = e.nombre;
            ");

            // 5. Ahora que todo está backfilleado, forzar NOT NULL, crear índices y FKs.
            migrationBuilder.AlterColumn<int>(
                name: "especie_id",
                table: "vaccines",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "especie_id",
                table: "mascotas",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_vaccines_especie_id",
                table: "vaccines",
                column: "especie_id");

            migrationBuilder.CreateIndex(
                name: "IX_mascotas_especie_id",
                table: "mascotas",
                column: "especie_id");

            migrationBuilder.AddForeignKey(
                name: "mascotas_ibfk_2",
                table: "mascotas",
                column: "especie_id",
                principalTable: "especies",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "vaccines_ibfk_1",
                table: "vaccines",
                column: "especie_id",
                principalTable: "especies",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            // 6. Quitar las columnas string viejas y el índice compuesto que dependía de ellas (sin callers hoy).
            migrationBuilder.DropIndex(
                name: "idx_Busqueda_mascotas",
                table: "mascotas");

            migrationBuilder.DropColumn(
                name: "species",
                table: "vaccines");

            migrationBuilder.DropColumn(
                name: "especie",
                table: "mascotas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "mascotas_ibfk_2",
                table: "mascotas");

            migrationBuilder.DropForeignKey(
                name: "vaccines_ibfk_1",
                table: "vaccines");

            migrationBuilder.DropIndex(
                name: "IX_vaccines_especie_id",
                table: "vaccines");

            migrationBuilder.DropIndex(
                name: "IX_mascotas_especie_id",
                table: "mascotas");

            migrationBuilder.AddColumn<string>(
                name: "species",
                table: "vaccines",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "especie",
                table: "mascotas",
                type: "text",
                nullable: true);

            // Restauración best-effort: recupera el texto a partir del catálogo. El casing/vocabulario
            // original exacto (ej. "perro" vs "canino") no es recuperable, ya se colapsó deliberadamente en Up().
            migrationBuilder.Sql(@"
                UPDATE vaccines v SET species = e.nombre FROM especies e WHERE v.especie_id = e.id;
                UPDATE mascotas m SET especie = e.nombre FROM especies e WHERE m.especie_id = e.id;
            ");

            migrationBuilder.AlterColumn<string>(
                name: "species",
                table: "vaccines",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "especie",
                table: "mascotas",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "especie_id",
                table: "vaccines");

            migrationBuilder.DropColumn(
                name: "especie_id",
                table: "mascotas");

            migrationBuilder.CreateIndex(
                name: "idx_Busqueda_mascotas",
                table: "mascotas",
                columns: new[] { "nombre", "especie", "raza" });

            migrationBuilder.DropTable(
                name: "especies");
        }
    }
}
