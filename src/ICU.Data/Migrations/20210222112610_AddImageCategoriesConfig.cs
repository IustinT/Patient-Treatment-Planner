using Microsoft.EntityFrameworkCore.Migrations;

namespace ICU.Data.Migrations
{
    public partial class AddImageCategoriesConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ID",
                table: "ImageCategories",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ImageCategories",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ImageCategories",
                newName: "ID");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ImageCategories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450);
        }
    }
}
