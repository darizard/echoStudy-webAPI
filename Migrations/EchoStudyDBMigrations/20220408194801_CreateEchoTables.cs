using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace echoStudy_webAPI.Migrations.EchoStudyDBMigrations
{
    public partial class CreateEchoTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeckCategories",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTouched = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeckCategories", x => x.CategoryID);
                });

            migrationBuilder.CreateTable(
                name: "Decks",
                columns: table => new
                {
                    DeckID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Access = table.Column<int>(type: "int", maxLength: 40, nullable: false),
                    DefaultFrontLang = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    DefaultBackLang = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTouched = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Decks", x => x.DeckID);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    SessionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayOrder = table.Column<int>(type: "int", nullable: false),
                    LearnReview = table.Column<int>(type: "int", nullable: false),
                    MaxCards = table.Column<int>(type: "int", nullable: false),
                    Platform = table.Column<int>(type: "int", nullable: false),
                    Device = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeckID = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateLastStudied = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.SessionID);
                });

            migrationBuilder.CreateTable(
                name: "DeckDeckCategory",
                columns: table => new
                {
                    DeckCategoriesCategoryID = table.Column<int>(type: "int", nullable: false),
                    DecksDeckID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeckDeckCategory", x => new { x.DeckCategoriesCategoryID, x.DecksDeckID });
                    table.ForeignKey(
                        name: "FK_DeckDeckCategory_DeckCategories_DeckCategoriesCategoryID",
                        column: x => x.DeckCategoriesCategoryID,
                        principalTable: "DeckCategories",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeckDeckCategory_Decks_DecksDeckID",
                        column: x => x.DecksDeckID,
                        principalTable: "Decks",
                        principalColumn: "DeckID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    CardID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FrontText = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    BackText = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FrontAudio = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    BackAudio = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    FrontLang = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    BackLang = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeckID = table.Column<int>(type: "int", nullable: false),
                    DeckPosition = table.Column<long>(type: "bigint", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTouched = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    SessionID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.CardID);
                    table.UniqueConstraint("AK_Cards_DeckID_DeckPosition", x => new { x.DeckID, x.DeckPosition });
                    table.ForeignKey(
                        name: "FK_Cards_Decks_DeckID",
                        column: x => x.DeckID,
                        principalTable: "Decks",
                        principalColumn: "DeckID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cards_Sessions_SessionID",
                        column: x => x.SessionID,
                        principalTable: "Sessions",
                        principalColumn: "SessionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_SessionID",
                table: "Cards",
                column: "SessionID");

            migrationBuilder.CreateIndex(
                name: "IX_DeckDeckCategory_DecksDeckID",
                table: "DeckDeckCategory",
                column: "DecksDeckID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "DeckDeckCategory");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "DeckCategories");

            migrationBuilder.DropTable(
                name: "Decks");
        }
    }
}
