using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    public partial class init4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
             name: "ExpirationDate",
             table: "Products",
             nullable: true,  // Set nullable to true
             oldNullable: false,  // Update the previous nullable value
             oldClrType: typeof(DateTime));
            migrationBuilder.AddColumn<bool>(
            name: "RequiresExpirationDate",
             table: "Products",
             nullable: false,
             defaultValue: false);
        }


        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the RequiresExpirationDate column.
            migrationBuilder.DropColumn(
                name: "RequiresExpirationDate",
                table: "Products");

            // Revert the ExpirationDate column to not nullable.
            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpirationDate",
                table: "Products",
                nullable: false,
                oldClrType: typeof(DateTime));
        }

    }
}
