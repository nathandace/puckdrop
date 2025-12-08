using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PuckDrop.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FavoriteTeams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TeamAbbrev = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    TeamName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LogoUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    PrimaryColor = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    SecondaryColor = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteTeams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcessedEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    EventId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EventType = table.Column<int>(type: "INTEGER", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessedEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebhookRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TeamAbbrev = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    EventType = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetUrl = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    PayloadFormat = table.Column<int>(type: "INTEGER", nullable: false),
                    IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CustomPayloadTemplate = table.Column<string>(type: "TEXT", nullable: true),
                    DelaySeconds = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebhookRules", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessedEvents_GameId_EventId",
                table: "ProcessedEvents",
                columns: new[] { "GameId", "EventId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessedEvents_ProcessedAt",
                table: "ProcessedEvents",
                column: "ProcessedAt");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookRules_IsEnabled_TeamAbbrev_EventType",
                table: "WebhookRules",
                columns: new[] { "IsEnabled", "TeamAbbrev", "EventType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriteTeams");

            migrationBuilder.DropTable(
                name: "ProcessedEvents");

            migrationBuilder.DropTable(
                name: "WebhookRules");
        }
    }
}
