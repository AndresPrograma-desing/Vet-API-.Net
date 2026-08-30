using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace vet_api_Net.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissionDefinitions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "permission_definitions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    label = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    creado = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permission_definitions", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_permission_definitions_key",
                table: "permission_definitions",
                column: "key",
                unique: true);

            // Seed: una fila por cada permissionKey hoy hardcodeado en Routes.jsx (frontend),
            // en el mismo orden en que aparecen ahí, para no perder ni renombrar ninguna clave
            // ya guardada en user_permissions de usuarios reales.
            migrationBuilder.Sql(@"
                INSERT INTO permission_definitions (key, label, icon, sort_order, creado) VALUES
                ('user.dashboard', 'Dashboard', 'house', 1, now()),
                ('user.projects', 'Proyectos', 'service', 2, now()),
                ('user.messages', 'Mensajes', 'message', 3, now()),
                ('admin.panel', 'Panel Administrativo', 'adminRole', 4, now()),
                ('admin.dashboard', 'Dashboard', 'dashboard', 5, now()),
                ('admin.users', 'Usuarios', 'user', 6, now()),
                ('admin.users.create', 'Crear usuario', 'UserPlus', 7, now()),
                ('admin.products.view', 'Productos - Ver productos', 'box', 8, now()),
                ('admin.products.manage', 'Productos - Administrar productos', 'box', 9, now()),
                ('admin.messages', 'Mensajes', 'message', 10, now()),
                ('admin.invoices.view', 'Ver Recibos', 'fileCheck', 11, now()),
                ('admin.notifications.create', 'Crear Ticket', 'BellPlus', 12, now()),
                ('admin.clients', 'Clientes', 'Users', 13, now()),
                ('admin.emails', 'Plantillas Emails', 'Mail', 14, now()),
                ('admin.vaccines.view', 'Vacunas - Ver vacunas', 'Activity', 15, now()),
                ('admin.vaccines.manage', 'Vacunas - Crear vacuna', 'Activity', 16, now()),
                ('admin.vaccines.pending', 'Vacunas - Vacunas pendientes', 'Activity', 17, now()),
                ('admin.vaccines.batches', 'Vacunas - Lotes de vacunas', 'Activity', 18, now()),
                ('admin.settings', 'Ajustes', 'engranaje', 19, now()),
                ('secretaria.panel', 'Panel', 'secretariaRole', 20, now()),
                ('secretaria.citas.list', 'Citas - Lista de Citas', 'calendario', 21, now()),
                ('secretaria.citas.create', 'Citas - Nueva Cita', 'calendario', 22, now()),
                ('secretaria.citas.newClient', 'Nuevo Cliente', 'Users', 23, now()),
                ('secretaria.calendar', 'Calendario', 'calendario', 24, now()),
                ('secretaria.invoices.manage', 'Gestion de Recibos', 'fileText', 25, now()),
                ('secretaria.messages', 'Mensajes', 'message', 26, now()),
                ('secretaria.invoices.view', 'Ver Recibos', 'fileCheck', 27, now()),
                ('secretaria.vaccines.view', 'Vacunas - Ver vacunas', 'Activity', 28, now()),
                ('secretaria.vaccines.manage', 'Vacunas - Crear vacuna', 'Activity', 29, now()),
                ('secretaria.vaccines.pending', 'Vacunas - Vacunas pendientes', 'Activity', 30, now()),
                ('secretaria.vaccines.batches', 'Vacunas - Lotes de vacunas', 'Activity', 31, now()),
                ('secretaria.recipes', 'Gestión de Recetas', 'clipboardPlus', 32, now()),
                ('secretaria.notifications.create', 'Crear Alerta', 'BellPlus', 33, now()),
                ('secretaria.clients', 'Clientes', 'Users', 34, now()),
                ('doctor.panel', 'Panel Médico', 'userCita', 35, now()),
                ('doctor.citas', 'Mis Citas', 'calendario', 36, now()),
                ('doctor.calendar', 'Calendario', 'calendario', 37, now()),
                ('doctor.records', 'Historias Clínicas', 'fileText', 38, now()),
                ('doctor.recipes', 'Gestión de Recetas', 'clipboardPlus', 39, now()),
                ('doctor.vaccines.view', 'Vacunas - Ver vacunas', 'Activity', 40, now()),
                ('doctor.vaccines.manage', 'Vacunas - Crear vacuna', 'Activity', 41, now()),
                ('doctor.vaccines.pending', 'Vacunas - Vacunas pendientes', 'Activity', 42, now()),
                ('doctor.vaccines.batches', 'Vacunas - Lotes de vacunas', 'Activity', 43, now()),
                ('doctor.messages', 'Mensajes', 'message', 44, now()),
                ('doctor.invoices.view', 'Ver Recibos', 'fileCheck', 45, now()),
                ('doctor.notifications.create', 'Crear Alerta', 'BellPlus', 46, now());
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "permission_definitions");
        }
    }
}
