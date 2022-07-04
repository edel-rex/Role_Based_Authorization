using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace jwt_employee.Migrations
{
    public partial class Added_ForgetPassword : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailValidationToken",
                table: "UserInfo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResetToken",
                table: "UserInfo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetTokenExpires",
                table: "UserInfo",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedAt",
                table: "UserInfo",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailValidationToken",
                table: "UserInfo");

            migrationBuilder.DropColumn(
                name: "ResetToken",
                table: "UserInfo");

            migrationBuilder.DropColumn(
                name: "ResetTokenExpires",
                table: "UserInfo");

            migrationBuilder.DropColumn(
                name: "VerifiedAt",
                table: "UserInfo");
        }
    }
}
