using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class LinkPassengerToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "LinePassengers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_LinePassengers_AppUserId",
                table: "LinePassengers",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LinePassengers_AspNetUsers_AppUserId",
                table: "LinePassengers",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LinePassengers_AspNetUsers_AppUserId",
                table: "LinePassengers");

            migrationBuilder.DropIndex(
                name: "IX_LinePassengers_AppUserId",
                table: "LinePassengers");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "LinePassengers");
        }
    }
}
