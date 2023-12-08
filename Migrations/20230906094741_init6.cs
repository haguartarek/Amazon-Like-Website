using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    public partial class init6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                  name: "Value",
                  table: "Orders",
                  type: "decimal(18,2)",
                  nullable: true);

            migrationBuilder.AddColumn<decimal>(
               name: "Value",
               table: "Carts",
               type: "decimal(18,2)",
               nullable: true);

                 
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
       table: "Orders",
       name: "Value");

         
            migrationBuilder.DropColumn(
                table: "Cart",
                name: "Value");

        }
    }
}
