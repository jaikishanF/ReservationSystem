using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebReservation.Migrations
{
    /// <inheritdoc />
    public partial class BuildingName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateChoiceString",
                table: "Choice");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Building",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Building");

            migrationBuilder.AddColumn<string>(
                name: "DateChoiceString",
                table: "Choice",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
