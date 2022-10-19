using Microsoft.EntityFrameworkCore.Migrations;

namespace echoStudy_webAPI.Migrations.EchoStudyDBMigrations
{
    public partial class AddBasicDeckSharing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "StudyPercent",
                table: "Decks",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<string>(
                name: "OrigAuthorId",
                table: "Decks",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrigDeckId",
                table: "Decks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Decks_OrigAuthorId",
                table: "Decks",
                column: "OrigAuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Decks_OrigDeckId",
                table: "Decks",
                column: "OrigDeckId");

            migrationBuilder.AddForeignKey(
                name: "FK_Decks_AspNetUsers_OrigAuthorId",
                table: "Decks",
                column: "OrigAuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Decks_Decks_OrigDeckId",
                table: "Decks",
                column: "OrigDeckId",
                principalTable: "Decks",
                principalColumn: "DeckID",
                onDelete: ReferentialAction.Restrict); // cannot SetNull on self-referencing foreign key. MSSQL limitation.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Decks_AspNetUsers_OrigAuthorId",
                table: "Decks");

            migrationBuilder.DropForeignKey(
                name: "FK_Decks_Decks_OrigDeckId",
                table: "Decks");

            migrationBuilder.DropIndex(
                name: "IX_Decks_OrigAuthorId",
                table: "Decks");

            migrationBuilder.DropIndex(
                name: "IX_Decks_OrigDeckId",
                table: "Decks");

            migrationBuilder.DropColumn(
                name: "OrigAuthorId",
                table: "Decks");

            migrationBuilder.DropColumn(
                name: "OrigDeckId",
                table: "Decks");

            migrationBuilder.AlterColumn<double>(
                name: "StudyPercent",
                table: "Decks",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);
        }
    }
}
