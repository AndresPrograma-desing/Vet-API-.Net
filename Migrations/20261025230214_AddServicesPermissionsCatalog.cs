using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vet_api_Net.Migrations
{
    /// <inheritdoc />
    public partial class AddServicesPermissionsCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed: módulos nuevos para las pantallas de Servicios (HU-26), Kardex de Inventario
            // (HU-13) e Historial de Precios (HU-14); más un permiso nuevo dentro del módulo
            // 'invoices' ya existente para la Cuenta de Cliente (HU-27), que vive como acción dentro
            // de la pantalla de clientes en vez de una pantalla propia en el menú lateral.
            migrationBuilder.Sql(@"
                INSERT INTO permission_modules (module_key, label, icon, sort_order, creado) VALUES
                ('services', 'Servicios', 'service', 15, now()),
                ('inventory', 'Inventario', 'box', 16, now()),
                ('priceHistory', 'Historial de Precios', 'fileClock', 17, now());

                INSERT INTO permission_definitions (module_id, key, label, sort_order, creado) VALUES
                ((SELECT id FROM permission_modules WHERE module_key = 'services'), 'services.view', 'Ver servicios', 24, now()),
                ((SELECT id FROM permission_modules WHERE module_key = 'services'), 'services.manage', 'Administrar servicios', 25, now()),
                ((SELECT id FROM permission_modules WHERE module_key = 'inventory'), 'inventory.view', 'Ver kardex de inventario', 26, now()),
                ((SELECT id FROM permission_modules WHERE module_key = 'priceHistory'), 'priceHistory.view', 'Ver historial de precios', 27, now()),
                ((SELECT id FROM permission_modules WHERE module_key = 'invoices'), 'invoices.account', 'Cuenta de cliente', 28, now());
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM permission_definitions WHERE key IN (
                    'services.view', 'services.manage', 'inventory.view', 'priceHistory.view', 'invoices.account'
                );
                DELETE FROM permission_modules WHERE module_key IN ('services', 'inventory', 'priceHistory');
            ");
        }
    }
}
