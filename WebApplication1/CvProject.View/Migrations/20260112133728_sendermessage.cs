using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CvProject.View.Migrations
{
    /// <inheritdoc />
    public partial class sendermessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SenderName",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SenderName",
                table: "Messages");
        }
    }
}
