using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiwesConnect.Migrations
{
    /// <inheritdoc />
    public partial class AddSupervisorIDToInternship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SupervisorID",
                table: "Internships",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupervisorID",
                table: "Internships");
        }
    }
}
