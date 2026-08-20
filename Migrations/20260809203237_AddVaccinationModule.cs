using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace vet_api_Net.Migrations
{
    /// <inheritdoc />
    public partial class AddVaccinationModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vacunas");

            migrationBuilder.CreateTable(
                name: "vaccines",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    species = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    minimum_age_weeks = table.Column<int>(type: "integer", nullable: false),
                    booster_frequency_value = table.Column<int>(type: "integer", nullable: false),
                    booster_frequency_unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    producto_id = table.Column<int>(type: "integer", nullable: true),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vaccines", x => x.id);
                    table.ForeignKey(
                        name: "vaccines_ibfk_1",
                        column: x => x.producto_id,
                        principalTable: "productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "vaccine_batches",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    vaccine_id = table.Column<int>(type: "integer", nullable: false),
                    laboratory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    batch_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    expiration_date = table.Column<DateOnly>(type: "date", nullable: false),
                    quantity_in_stock = table.Column<int>(type: "integer", nullable: false),
                    received_date = table.Column<DateOnly>(type: "date", nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vaccine_batches", x => x.id);
                    table.ForeignKey(
                        name: "vaccine_batches_ibfk_1",
                        column: x => x.vaccine_id,
                        principalTable: "vaccines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vaccine_scheme_stages",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    vaccine_id = table.Column<int>(type: "integer", nullable: false),
                    stage_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    dose_number = table.Column<int>(type: "integer", nullable: false),
                    interval_days_from_previous = table.Column<int>(type: "integer", nullable: false),
                    minimum_age_weeks_override = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vaccine_scheme_stages", x => x.id);
                    table.ForeignKey(
                        name: "vaccine_scheme_stages_ibfk_1",
                        column: x => x.vaccine_id,
                        principalTable: "vaccines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pet_vaccinations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mascota_id = table.Column<int>(type: "integer", nullable: false),
                    vaccine_id = table.Column<int>(type: "integer", nullable: false),
                    vaccine_batch_id = table.Column<int>(type: "integer", nullable: true),
                    consulta_id = table.Column<int>(type: "integer", nullable: true),
                    doctor_id = table.Column<int>(type: "integer", nullable: true),
                    application_date = table.Column<DateOnly>(type: "date", nullable: false),
                    weight_at_application = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    dose = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    next_dose_date = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    postponement_reason = table.Column<string>(type: "text", nullable: true),
                    clinical_observations = table.Column<string>(type: "text", nullable: true),
                    detalle_factura_id = table.Column<int>(type: "integer", nullable: true),
                    reminder_seven_sent_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    reminder_three_sent_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pet_vaccinations", x => x.id);
                    table.ForeignKey(
                        name: "pet_vaccinations_ibfk_1",
                        column: x => x.mascota_id,
                        principalTable: "mascotas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "pet_vaccinations_ibfk_2",
                        column: x => x.vaccine_id,
                        principalTable: "vaccines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "pet_vaccinations_ibfk_3",
                        column: x => x.vaccine_batch_id,
                        principalTable: "vaccine_batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "pet_vaccinations_ibfk_4",
                        column: x => x.consulta_id,
                        principalTable: "consultas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "pet_vaccinations_ibfk_5",
                        column: x => x.doctor_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "pet_vaccinations_ibfk_6",
                        column: x => x.detalle_factura_id,
                        principalTable: "detalles_factura",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "consulta_id1",
                table: "pet_vaccinations",
                column: "consulta_id");

            migrationBuilder.CreateIndex(
                name: "detalle_factura_id",
                table: "pet_vaccinations",
                column: "detalle_factura_id");

            migrationBuilder.CreateIndex(
                name: "doctor_id1",
                table: "pet_vaccinations",
                column: "doctor_id");

            migrationBuilder.CreateIndex(
                name: "idx_pet_vaccination_mascota",
                table: "pet_vaccinations",
                column: "mascota_id");

            migrationBuilder.CreateIndex(
                name: "idx_pet_vaccination_next_dose",
                table: "pet_vaccinations",
                column: "next_dose_date");

            migrationBuilder.CreateIndex(
                name: "vaccine_batch_id",
                table: "pet_vaccinations",
                column: "vaccine_batch_id");

            migrationBuilder.CreateIndex(
                name: "vaccine_id",
                table: "pet_vaccinations",
                column: "vaccine_id");

            migrationBuilder.CreateIndex(
                name: "idx_batch_vaccine_expiration",
                table: "vaccine_batches",
                columns: new[] { "vaccine_id", "expiration_date" });

            migrationBuilder.CreateIndex(
                name: "idx_scheme_stage_vaccine",
                table: "vaccine_scheme_stages",
                column: "vaccine_id");

            migrationBuilder.CreateIndex(
                name: "idx_vaccine_producto",
                table: "vaccines",
                column: "producto_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pet_vaccinations");

            migrationBuilder.DropTable(
                name: "vaccine_scheme_stages");

            migrationBuilder.DropTable(
                name: "vaccine_batches");

            migrationBuilder.DropTable(
                name: "vaccines");

            migrationBuilder.CreateTable(
                name: "vacunas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    consulta_id = table.Column<int>(type: "integer", nullable: true),
                    doctor_id = table.Column<int>(type: "integer", nullable: true),
                    mascota_id = table.Column<int>(type: "integer", nullable: false),
                    producto_id = table.Column<int>(type: "integer", nullable: false),
                    creado = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    fecha_vacunacion = table.Column<DateOnly>(type: "date", nullable: false),
                    lote = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    nota = table.Column<string>(type: "text", nullable: true),
                    proxima_dosis = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vacunas", x => x.id);
                    table.ForeignKey(
                        name: "vacunas_ibfk_1",
                        column: x => x.mascota_id,
                        principalTable: "mascotas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "vacunas_ibfk_2",
                        column: x => x.producto_id,
                        principalTable: "productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "vacunas_ibfk_3",
                        column: x => x.consulta_id,
                        principalTable: "consultas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "vacunas_ibfk_4",
                        column: x => x.doctor_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "consulta_id1",
                table: "vacunas",
                column: "consulta_id");

            migrationBuilder.CreateIndex(
                name: "doctor_id1",
                table: "vacunas",
                column: "doctor_id");

            migrationBuilder.CreateIndex(
                name: "idx_fecha_vacuna",
                table: "vacunas",
                column: "fecha_vacunacion");

            migrationBuilder.CreateIndex(
                name: "idx_mascota_vacuna",
                table: "vacunas",
                column: "mascota_id");

            migrationBuilder.CreateIndex(
                name: "producto_id2",
                table: "vacunas",
                column: "producto_id");
        }
    }
}
