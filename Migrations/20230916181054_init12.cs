using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    public partial class init12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
               name: "CategoryDiscounts",
               columns: table => new
               {
                   CategoryDiscountId = table.Column<int>(nullable: false)
                       .Annotation("SqlServer:Identity", "1, 1"),
                   CategoryId = table.Column<int>(nullable: false),
                   DiscountPercentage = table.Column<decimal>(nullable: false)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_CategoryDiscounts", x => x.CategoryDiscountId);
                   table.ForeignKey(
                       name: "FK_CategoryDiscounts_Categories_CategoryId",
                       column: x => x.CategoryId,
                       principalTable: "Categories",
                       principalColumn: "CategoryId",
                       onDelete: ReferentialAction.Cascade);
               });

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Cart_Productss",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Cart_Productss_CategoryId",
                table: "Cart_Productss",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryDiscounts_CategoryId",
                table: "CategoryDiscounts",
                column: "CategoryId");
        


    }

    protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
             name: "IX_Cart_Productss_CategoryId",
             table: "Cart_Productss");

            migrationBuilder.DropIndex(
                name: "IX_CategoryDiscounts_CategoryId",
                table: "CategoryDiscounts");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Cart_Productss");

            migrationBuilder.DropTable(
                name: "CategoryDiscounts");

        }
    }
}
