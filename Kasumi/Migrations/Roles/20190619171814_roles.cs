using Microsoft.EntityFrameworkCore.Migrations;

namespace Kasumi.Migrations.Roles
{
    public partial class roles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssignableRoles",
                columns: table => new
                {
                    RoleId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ServerId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignableRoles", x => x.RoleId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignableRoles");
        }
    }
}
