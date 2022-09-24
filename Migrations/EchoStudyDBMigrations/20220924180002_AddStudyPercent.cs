using Microsoft.EntityFrameworkCore.Migrations;

namespace echoStudy_webAPI.Migrations.EchoStudyDBMigrations
{
    public partial class AddStudyPercent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "StudyPercent",
                table: "Decks",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudyPercent",
                table: "Decks");
        }
    }
}
