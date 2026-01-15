using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class FixDeleteLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Buses_Lines_LineId",
                table: "Buses");

            migrationBuilder.DropForeignKey(
                name: "FK_LinePassengers_Lines_LineId",
                table: "LinePassengers");

            migrationBuilder.DropIndex(
                name: "IX_Buses_LineId",
                table: "Buses");

            migrationBuilder.AlterColumn<Guid>(
                name: "LineId",
                table: "Buses",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Buses_LineId",
                table: "Buses",
                column: "LineId",
                unique: true,
                filter: "[LineId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Buses_Lines_LineId",
                table: "Buses",
                column: "LineId",
                principalTable: "Lines",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LinePassengers_Lines_LineId",
                table: "LinePassengers",
                column: "LineId",
                principalTable: "Lines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Buses_Lines_LineId",
                table: "Buses");

            migrationBuilder.DropForeignKey(
                name: "FK_LinePassengers_Lines_LineId",
                table: "LinePassengers");

            migrationBuilder.DropIndex(
                name: "IX_Buses_LineId",
                table: "Buses");

            migrationBuilder.AlterColumn<Guid>(
                name: "LineId",
                table: "Buses",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Buses_LineId",
                table: "Buses",
                column: "LineId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Buses_Lines_LineId",
                table: "Buses",
                column: "LineId",
                principalTable: "Lines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LinePassengers_Lines_LineId",
                table: "LinePassengers",
                column: "LineId",
                principalTable: "Lines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
