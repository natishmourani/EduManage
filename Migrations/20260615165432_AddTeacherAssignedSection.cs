using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduManage.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherAssignedSection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedSection",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedSection",
                table: "Teachers");
        }
    }
}
