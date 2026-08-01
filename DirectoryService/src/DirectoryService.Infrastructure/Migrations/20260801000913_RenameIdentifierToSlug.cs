using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameIdentifierToSlug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "identifier",
                table: "departments",
                newName: "slug");

            migrationBuilder.RenameIndex(
                name: "IX_departments_identifier",
                table: "departments",
                newName: "IX_departments_slug");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "slug",
                table: "departments",
                newName: "identifier");

            migrationBuilder.RenameIndex(
                name: "IX_departments_slug",
                table: "departments",
                newName: "IX_departments_identifier");
        }
    }
}
