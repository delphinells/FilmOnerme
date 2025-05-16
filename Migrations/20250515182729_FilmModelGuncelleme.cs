using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilmOnerme.Migrations
{
    /// <inheritdoc />
    public partial class FilmModelGuncelleme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilmTuru",
                table: "Films",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ImdbPuani",
                table: "Films",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "YapimYili",
                table: "Films",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Yonetmen",
                table: "Films",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilmTuru",
                table: "Films");

            migrationBuilder.DropColumn(
                name: "ImdbPuani",
                table: "Films");

            migrationBuilder.DropColumn(
                name: "YapimYili",
                table: "Films");

            migrationBuilder.DropColumn(
                name: "Yonetmen",
                table: "Films");
        }
    }
}
