using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class AddCommentsUserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "QuestionComments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "AnswerComments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "QuestionComments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AnswerComments");
        }
    }
}
