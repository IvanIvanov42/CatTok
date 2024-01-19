using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Expatery_API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMediaModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "username",
                table: "Media");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "username",
                table: "Media",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
