﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SitoLtb.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorAndDescriptionToPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorName",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorName",
                table: "Posts");
        }
    }
}
