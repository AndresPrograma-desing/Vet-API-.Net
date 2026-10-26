using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace vet_api_Net.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkerProcessEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "worker_process_events",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name_event = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    type_event = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    error_message = table.Column<string>(type: "text", nullable: true),
                    vinculation_data = table.Column<string>(type: "text", nullable: true),
                    intents = table.Column<int>(type: "integer", nullable: false),
                    is_resolved = table.Column<bool>(type: "boolean", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    resolved_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_worker_process_events", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_created_worker_process",
                table: "worker_process_events",
                column: "created");

            migrationBuilder.CreateIndex(
                name: "idx_is_resolved_worker_process",
                table: "worker_process_events",
                column: "is_resolved");

            migrationBuilder.CreateIndex(
                name: "idx_name_event_worker_process",
                table: "worker_process_events",
                column: "name_event");

            migrationBuilder.CreateIndex(
                name: "idx_type_event_worker_process",
                table: "worker_process_events",
                column: "type_event");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "worker_process_events");
        }
    }
}
