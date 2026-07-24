using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vet_api_Net.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatarUrlToUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "usuarios",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "usuarios");
        }
    }
}
