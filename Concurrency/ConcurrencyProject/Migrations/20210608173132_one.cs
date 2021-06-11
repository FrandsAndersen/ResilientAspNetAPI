using Microsoft.EntityFrameworkCore.Migrations;

namespace ConcurrencyProject.Migrations
{
    public partial class one : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NbrOfPages",
                table: "Books");

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Books");

            migrationBuilder.AddColumn<int>(
                name: "NbrOfPages",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
