using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class CreatedDbSetForQuestionTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionTag_Questions_QuestionId",
                table: "QuestionTag");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionTag_Tags_TagId",
                table: "QuestionTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionTag",
                table: "QuestionTag");

            migrationBuilder.RenameTable(
                name: "QuestionTag",
                newName: "QuestionTags");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionTag_TagId",
                table: "QuestionTags",
                newName: "IX_QuestionTags_TagId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionTag_QuestionId",
                table: "QuestionTags",
                newName: "IX_QuestionTags_QuestionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionTags",
                table: "QuestionTags",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionTags_Questions_QuestionId",
                table: "QuestionTags",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionTags_Tags_TagId",
                table: "QuestionTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionTags_Questions_QuestionId",
                table: "QuestionTags");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionTags_Tags_TagId",
                table: "QuestionTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionTags",
                table: "QuestionTags");

            migrationBuilder.RenameTable(
                name: "QuestionTags",
                newName: "QuestionTag");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionTags_TagId",
                table: "QuestionTag",
                newName: "IX_QuestionTag_TagId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionTags_QuestionId",
                table: "QuestionTag",
                newName: "IX_QuestionTag_QuestionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionTag",
                table: "QuestionTag",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionTag_Questions_QuestionId",
                table: "QuestionTag",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionTag_Tags_TagId",
                table: "QuestionTag",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
