using Microsoft.EntityFrameworkCore.Migrations;

namespace PipelineCacher.Entities.Migrations
{
    public partial class Change : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CommitId",
                table: "Sourcecode",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProviderType",
                table: "Sourcecode",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RepositoryId",
                table: "Sourcecode",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommitId",
                table: "PipelineContext",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommitId",
                table: "Sourcecode");

            migrationBuilder.DropColumn(
                name: "ProviderType",
                table: "Sourcecode");

            migrationBuilder.DropColumn(
                name: "RepositoryId",
                table: "Sourcecode");

            migrationBuilder.DropColumn(
                name: "CommitId",
                table: "PipelineContext");
        }
    }
}
