using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SitoLtb.Migrations
{
    /// <inheritdoc />
    public partial class Swag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Elo",
                table: "Tournaments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Elo",
                table: "Tournaments");
        }
    }
}
