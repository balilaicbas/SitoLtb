using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SitoLtb.Migrations
{
    /// <inheritdoc />
    public partial class AddTournamentSede : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sede",
                table: "Tournaments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sede",
                table: "Tournaments");
        }
    }
}
