using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cattok_API.Migrations
{
    /// <inheritdoc />
    public partial class Media : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Media",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    media_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    media_url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    caption = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    time_stamp = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Media");
        }
    }
}