using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    public partial class init9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
              name: "ActivationRequests",
              columns: table => new
              {
                  ActivationRequestId = table.Column<int>(type: "int", nullable: false)
                      .Annotation("SqlServer:Identity", "1, 1"),
                  RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                  IsActive = table.Column<bool>(type: "bit", nullable: false)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_ActivationRequests", x => x.ActivationRequestId);
              });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
          name: "ActivationRequests");

        }
    }
}
