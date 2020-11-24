using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class RenameQuestionImagePathColumnToImageNamePath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Questions");

            migrationBuilder.AddColumn<string>(
                name: "ImageNamePath",
                table: "Questions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageNamePath",
                table: "Questions");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
