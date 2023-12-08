using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    public partial class init13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CatProductPrice",
                table: "Cart_Productss",
                type: "decimal",
                nullable: true,
                defaultValue: 0);

           
               
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.DropColumn(
                name: "CatProductPrice",
                table: "Cart_Productss");
        }
    }
}
