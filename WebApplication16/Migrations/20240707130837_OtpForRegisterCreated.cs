using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication16.Migrations
{
    public partial class OtpForRegisterCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OTPtimerRegister",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OTPtimerRegister",
                table: "AspNetUsers");
        }
    }
}
