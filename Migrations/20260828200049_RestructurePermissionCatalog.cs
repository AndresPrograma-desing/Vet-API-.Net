using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace vet_api_Net.Migrations
{
    /// <inheritdoc />
    public partial class RestructurePermissionCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Reset: los datos de permission_definitions/user_permissions son de desarrollo y usaban
            // keys con el rol embebido (ej. "admin.invoices.view"); se limpian antes de restructurar
            // el catálogo en módulos con keys globales, en vez de migrarlos key por key.
            migrationBuilder.Sql("DELETE FROM permission_definitions;");
            migrationBuilder.Sql("DELETE FROM user_permissions;");

            migrationBuilder.DropColumn(
                name: "icon",
                table: "permission_definitions");

            migrationBuilder.AddColumn<int>(
                name: "module_id",
                table: "permission_definitions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "permission_modules",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    module_key = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    label = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    creado = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permission_modules", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_permission_definitions_module_id",
                table: "permission_definitions",
                column: "module_id");

            migrationBuilder.CreateIndex(
                name: "idx_permission_modules_module_key",
                table: "permission_modules",
                column: "module_key",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_permission_definitions_module",
                table: "permission_definitions",
                column: "module_id",
                principalTable: "permission_modules",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            // Seed: catálogo de permisos agrupado por módulo, con keys globales (sin rol embebido).
            // Reemplaza las 46 filas planas (rol.accion) de la migración AddPermissionDefinitions,
            // deduplicando permisos idénticos repetidos por rol (ej. admin/secretaria/doctor.invoices.view
            // eran el mismo permiso "Ver Recibos" tres veces). Las entradas *.panel/*.dashboard no se
            // migran: representan el acceso implícito a la pantalla de inicio de cada Usuario.Rol.
            migrationBuilder.Sql(@"
                INSERT INTO permission_modules (module_key, label, icon, sort_order, creado) VALUES
                ('projects', 'Proyectos', 'service', 1, now()),
                ('messages', 'Mensajes', 'message', 2, now()),
                ('users', 'Usuarios', 'user', 3, now()),
                ('products', 'Productos', 'box', 4, now()),
                ('invoices', 'Facturación y Recibos', 'fileCheck', 5, now()),
                ('notifications', 'Notificaciones', 'BellPlus', 6, now()),
                ('clients', 'Clientes', 'Users', 7, now()),
                ('emails', 'Plantillas de Email', 'Mail', 8, now()),
                ('vaccines', 'Vacunas', 'Activity', 9, now()),
                ('settings', 'Ajustes', 'engranaje', 10, now()),
                ('citas', 'Citas', 'calendario', 11, now()),
                ('calendar', 'Calendario', 'calendario', 12, now()),
                ('recipes', 'Recetas', 'clipboardPlus', 13, now()),
                ('records', 'Historias Clínicas', 'fileText', 14, now());

                INSERT INTO permission_definitions (module_id, key, label, sort_order, creado) VALUES
                ((SELECT id FROM permission_modules WHERE module_key = 'projects'), 'projects.view', 'Proyectos', 1, now()),
                ((SELECT id FROM permission_modules WHERE module_key = 'messages'), 'messages.view', 'Mensajes', 2, now()),
                ((SELECT id FROM permission_modules WHERE module_key = 'users'), 'users.view', 'Usuarios', 3, now()),
                ((SELECT id FROM permission_modules WHERE module_key = 'users'), 'users.create', 'Crear usuario', 4, now()),
                ((SELECT id FROM permission_modules WHERE module_key = 'products'), 'products.view', 'Ver productos', 5, now()),
                ((SELECT id FROM permission_modules WHERE module_key = 'products'), 'products.manage', 'Administrar productos', 6, now()),
                ((SELECT id FROM permission_modules WHERE module_key = 'invoices'), 'invoices.view', 'Ver recibos', 7, now()),
                ((SELECT id FROM permission_modules WHERE module_key = 'invoices'), 'invoices.manage', 'Gestión de recibos', 8, now()),
                ((SELECT id FROM permission_modules WHERE module_key = 'notifications'), 'notifications.create', 'Crear Alerta', 9, now()),
                ((SELECT id FROM permission_modules WHERE module_key = 'clients'), 'clients.view', 'Clientes', 10, now()),
                ((SELECT id FROM permission_modules WHERE module_key = 'clients'), 'clients.create', 'Nuevo Cliente', 11, now()),
                ((SELECT id FROM permission_modules WHERE module_key = 'emails'), 'emails.manage', 'Plantillas Emails', 12, now()),
                ((SELECT id FROM permission_modules WHERE module_key = 'vaccines'), 'vaccines.view', 'Ver vacunas', 13, now()),
                ((SELECT id FROM permission_modules WHERE module_key = 'vaccines'), 'vaccines.manage', 'Crear vacuna', 14, now()),
                ((SELECT id FROM permission_modules WHERE module_key = 'vaccines'), 'vaccines.pending', 'Vacunas pendientes', 15, now()),
                ((SELECT id FROM permission_modules WHERE module_key = 'vaccines'), 'vaccines.batches', 'Lotes de vacunas', 16, now()),
                ((SELECT id FROM permission_modules WHERE module_key = 'settings'), 'settings.manage', 'Ajustes', 17, now()),
                ((SELECT id FROM permission_modules WHERE module_key = 'citas'), 'citas.list', 'Lista de Citas', 18, now()),
                ((SELECT id FROM permission_modules WHERE module_key = 'citas'), 'citas.create', 'Nueva Cita', 19, now()),
                ((SELECT id FROM permission_modules WHERE module_key = 'citas'), 'citas.mine', 'Mis Citas', 20, now()),
                ((SELECT id FROM permission_modules WHERE module_key = 'calendar'), 'calendar.view', 'Calendario', 21, now()),
                ((SELECT id FROM permission_modules WHERE module_key = 'recipes'), 'recipes.manage', 'Gestión de Recetas', 22, now()),
                ((SELECT id FROM permission_modules WHERE module_key = 'records'), 'records.view', 'Historias Clínicas', 23, now());
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_permission_definitions_module",
                table: "permission_definitions");

            migrationBuilder.DropTable(
                name: "permission_modules");

            migrationBuilder.DropIndex(
                name: "IX_permission_definitions_module_id",
                table: "permission_definitions");

            migrationBuilder.DropColumn(
                name: "module_id",
                table: "permission_definitions");

            migrationBuilder.AddColumn<string>(
                name: "icon",
                table: "permission_definitions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
