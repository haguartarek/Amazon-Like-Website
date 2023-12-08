using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    public partial class init7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
               name: "OldProduct",
               columns: table => new
               {
                   OldProductId = table.Column<int>(nullable: false)
                       .Annotation("SqlServer:Identity", "1, 1"),
                   Name = table.Column<string>(nullable: true),
                   
                   Quantity = table.Column<int>(nullable: false),
                   Price = table.Column<decimal>(nullable: false),
                   Description = table.Column<string>(nullable: true),
                   CategoryId = table.Column<int>(nullable: false),
                   ExpirationDate = table.Column<DateTime>(nullable: true),
                   IsActive = table.Column<bool>(nullable: false),
                   Image = table.Column<byte[]>(nullable: true),
                   RequiresExpirationDate = table.Column<bool>(nullable: false)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_OldProduct", x => x.OldProductId);
              
                  
               });

            migrationBuilder.CreateIndex(
                name: "IX_OldProduct_CategoryId",
                table: "OldProduct",
                column: "CategoryId");

         

       
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OldProduct");

           


        }
    }
}
