using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctorConnect.Migrations
{
    /// <inheritdoc />
    public partial class DurationInMinutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DurationInMinutes",
                table: "DoctorAvailabilities",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationInMinutes",
                table: "DoctorAvailabilities");
        }
    }
}
