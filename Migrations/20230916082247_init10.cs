using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    public partial class init10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SellerId",
                table: "ActivationRequests",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ActivationRequests_SellerId",
                table: "ActivationRequests",
                column: "SellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivationRequests_AspNetUsers_SellerId",
                table: "ActivationRequests",
                column: "SellerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
              name: "FK_ActivationRequests_AspNetUsers_SellerId",
              table: "ActivationRequests");

            migrationBuilder.DropIndex(
                name: "IX_ActivationRequests_SellerId",
                table: "ActivationRequests");

            migrationBuilder.DropColumn(
                name: "SellerId",
                table: "ActivationRequests");

        }
    }
}
