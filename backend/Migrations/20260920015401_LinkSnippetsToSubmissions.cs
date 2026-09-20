using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class LinkSnippetsToSubmissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SnippetId",
                table: "Submissions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_SnippetId",
                table: "Submissions",
                column: "SnippetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Snippets_SnippetId",
                table: "Submissions",
                column: "SnippetId",
                principalTable: "Snippets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Snippets_SnippetId",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_SnippetId",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "SnippetId",
                table: "Submissions");
        }
    }
}
