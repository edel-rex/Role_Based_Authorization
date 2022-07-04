using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace jwt_employee.Migrations
{
    public partial class Added_Role : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Auth_Role",
                table: "UserInfo",
                type: "varchar(max)",
                unicode: false,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Auth_Role",
                table: "Employee",
                type: "varchar(max)",
                unicode: false,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Auth_Role",
                table: "UserInfo");

            migrationBuilder.DropColumn(
                name: "Auth_Role",
                table: "Employee");
        }
    }
}
