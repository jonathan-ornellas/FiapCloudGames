using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fiap.Game.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class libraryConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Library",
                table: "Library");

            migrationBuilder.DropIndex(
                name: "IX_Library_UserId_GameId",
                table: "Library");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Library",
                table: "Library",
                columns: new[] { "UserId", "GameId" });

            migrationBuilder.CreateIndex(
                name: "IX_Library_UserId",
                table: "Library",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Library",
                table: "Library");

            migrationBuilder.DropIndex(
                name: "IX_Library_UserId",
                table: "Library");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Library",
                table: "Library",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Library_UserId_GameId",
                table: "Library",
                columns: new[] { "UserId", "GameId" },
                unique: true);
        }
    }
}
