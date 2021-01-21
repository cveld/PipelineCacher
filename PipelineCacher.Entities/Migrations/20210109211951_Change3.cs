using Microsoft.EntityFrameworkCore.Migrations;

namespace PipelineCacher.Entities.Migrations
{
    public partial class Change3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Branch",
                table: "PipelineState",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Branch",
                table: "PipelineState");
        }
    }
}
