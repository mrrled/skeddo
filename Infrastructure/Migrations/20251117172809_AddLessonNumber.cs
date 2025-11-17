using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLessonNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Teachers",
                newName: "Surname");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "Teachers",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Surname",
                table: "Teachers",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Teachers",
                newName: "FirstName");
        }
    }
}
