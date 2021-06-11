using Microsoft.EntityFrameworkCore.Migrations;

namespace ConcurrencyProject.Migrations
{
    public partial class height : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "height",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "height",
                table: "Users");
        }
    }
}
