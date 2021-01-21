using Microsoft.EntityFrameworkCore.Migrations;

namespace PipelineCacher.Entities.Migrations
{
    public partial class Change6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SourcecodeTree",
                table: "PipelineState",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Revision",
                table: "Pipelines",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourcecodeTree",
                table: "PipelineState");

            migrationBuilder.DropColumn(
                name: "Revision",
                table: "Pipelines");
        }
    }
}
