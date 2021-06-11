using Microsoft.EntityFrameworkCore.Migrations;

namespace ConcurrencyProject.Migrations
{
    public partial class HeightCapital : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "height",
                table: "Users",
                newName: "Height");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Height",
                table: "Users",
                newName: "height");
        }
    }
}
