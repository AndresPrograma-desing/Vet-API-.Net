using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vet_api_Net.Migrations
{
    /// <inheritdoc />
    public partial class AddClientePortalLoginCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PortalLoginCode",
                table: "clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PortalLoginCodeExpireDate",
                table: "clientes",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PortalLoginCode",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "PortalLoginCodeExpireDate",
                table: "clientes");
        }
    }
}
