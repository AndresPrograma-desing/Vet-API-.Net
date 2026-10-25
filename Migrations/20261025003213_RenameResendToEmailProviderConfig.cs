using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vet_api_Net.Migrations
{
    /// <inheritdoc />
    public partial class RenameResendToEmailProviderConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "resend_from_email",
                table: "system_configs",
                newName: "email_provider_from_email");

            migrationBuilder.RenameColumn(
                name: "resend_api_url",
                table: "system_configs",
                newName: "email_provider_api_url");

            migrationBuilder.RenameColumn(
                name: "resend_api_key",
                table: "system_configs",
                newName: "email_provider_api_key");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "email_provider_from_email",
                table: "system_configs",
                newName: "resend_from_email");

            migrationBuilder.RenameColumn(
                name: "email_provider_api_url",
                table: "system_configs",
                newName: "resend_api_url");

            migrationBuilder.RenameColumn(
                name: "email_provider_api_key",
                table: "system_configs",
                newName: "resend_api_key");
        }
    }
}
