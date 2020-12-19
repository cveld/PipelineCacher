using Microsoft.EntityFrameworkCore.Migrations;

namespace PipelineCacher.Entities.Migrations
{
    public partial class Change3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AzdoId",
                table: "Pipelines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationName",
                table: "Pipelines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectName",
                table: "Pipelines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectEntityId = table.Column<int>(type: "int", nullable: true),
                    SubjectEntityName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ObjectEntityId = table.Column<int>(type: "int", nullable: true),
                    ObjectEntityName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLog");

            migrationBuilder.DropColumn(
                name: "AzdoId",
                table: "Pipelines");

            migrationBuilder.DropColumn(
                name: "OrganizationName",
                table: "Pipelines");

            migrationBuilder.DropColumn(
                name: "ProjectName",
                table: "Pipelines");
        }
    }
}
