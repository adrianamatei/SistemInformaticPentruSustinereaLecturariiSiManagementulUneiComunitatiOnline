using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AplicatieLicenta.Migrations
{
    /// <inheritdoc />
    public partial class SchimbaUserIdDinStringInInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RezultateQuiz_Users_UtilizatorIdUtilizator",
                table: "RezultateQuiz");

            migrationBuilder.DropIndex(
                name: "IX_RezultateQuiz_UtilizatorIdUtilizator",
                table: "RezultateQuiz");

            migrationBuilder.DropColumn(
                name: "UtilizatorIdUtilizator",
                table: "RezultateQuiz");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "RezultateQuiz",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_RezultateQuiz_UserId",
                table: "RezultateQuiz",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RezultateQuiz_Users_UserId",
                table: "RezultateQuiz",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "id_utilizator",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RezultateQuiz_Users_UserId",
                table: "RezultateQuiz");

            migrationBuilder.DropIndex(
                name: "IX_RezultateQuiz_UserId",
                table: "RezultateQuiz");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "RezultateQuiz",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "UtilizatorIdUtilizator",
                table: "RezultateQuiz",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RezultateQuiz_UtilizatorIdUtilizator",
                table: "RezultateQuiz",
                column: "UtilizatorIdUtilizator");

            migrationBuilder.AddForeignKey(
                name: "FK_RezultateQuiz_Users_UtilizatorIdUtilizator",
                table: "RezultateQuiz",
                column: "UtilizatorIdUtilizator",
                principalTable: "Users",
                principalColumn: "id_utilizator",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
