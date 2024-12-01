using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningStarter.Migrations
{
    /// <inheritdoc />
    public partial class newmigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServerChannelId",
                table: "Posts");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Posts");

            migrationBuilder.AddColumn<int>(
                name: "ServerChannelId",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
