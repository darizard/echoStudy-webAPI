using System;
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

            migrationBuilder.CreateTable(
                name: "DeckShares",
                columns: table => new
                {
                    ClonedDeckId = table.Column<int>(type: "int", nullable: false),
                    SourceDeckId = table.Column<int>(type: "int", nullable: true),
                    OrigAuthorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ShareDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeckShares", x => x.ClonedDeckId);
                    table.ForeignKey(
                        name: "FK_DeckShares_AspNetUsers_OrigAuthorId",
                        column: x => x.OrigAuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeckShares_Decks_ClonedDeckId",
                        column: x => x.ClonedDeckId,
                        principalTable: "Decks",
                        principalColumn: "DeckID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeckShares_Decks_SourceDeckId",
                        column: x => x.SourceDeckId,
                        principalTable: "Decks",
                        principalColumn: "DeckID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeckShares_OrigAuthorId",
                table: "DeckShares",
                column: "OrigAuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_DeckShares_SourceDeckId",
                table: "DeckShares",
                column: "SourceDeckId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeckShares");

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
