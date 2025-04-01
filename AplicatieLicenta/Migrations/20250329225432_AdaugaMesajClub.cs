using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AplicatieLicenta.Migrations
{
    /// <inheritdoc />
    public partial class AdaugaMesajClub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MesajeClub_CluburiLectura_IdClub",
                table: "MesajeClub");

            migrationBuilder.DropForeignKey(
                name: "FK_MesajeClub_Users_IdUtilizator",
                table: "MesajeClub");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MesajeClub",
                table: "MesajeClub");

            migrationBuilder.RenameTable(
                name: "MesajeClub",
                newName: "MesajClub");

            migrationBuilder.RenameIndex(
                name: "IX_MesajeClub_IdUtilizator",
                table: "MesajClub",
                newName: "IX_MesajClub_IdUtilizator");

            migrationBuilder.RenameIndex(
                name: "IX_MesajeClub_IdClub",
                table: "MesajClub",
                newName: "IX_MesajClub_IdClub");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MesajClub",
                table: "MesajClub",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MesajClub_CluburiLectura_IdClub",
                table: "MesajClub",
                column: "IdClub",
                principalTable: "CluburiLectura",
                principalColumn: "id_club",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MesajClub_Users_IdUtilizator",
                table: "MesajClub",
                column: "IdUtilizator",
                principalTable: "Users",
                principalColumn: "id_utilizator",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MesajClub_CluburiLectura_IdClub",
                table: "MesajClub");

            migrationBuilder.DropForeignKey(
                name: "FK_MesajClub_Users_IdUtilizator",
                table: "MesajClub");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MesajClub",
                table: "MesajClub");

            migrationBuilder.RenameTable(
                name: "MesajClub",
                newName: "MesajeClub");

            migrationBuilder.RenameIndex(
                name: "IX_MesajClub_IdUtilizator",
                table: "MesajeClub",
                newName: "IX_MesajeClub_IdUtilizator");

            migrationBuilder.RenameIndex(
                name: "IX_MesajClub_IdClub",
                table: "MesajeClub",
                newName: "IX_MesajeClub_IdClub");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MesajeClub",
                table: "MesajeClub",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MesajeClub_CluburiLectura_IdClub",
                table: "MesajeClub",
                column: "IdClub",
                principalTable: "CluburiLectura",
                principalColumn: "id_club",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MesajeClub_Users_IdUtilizator",
                table: "MesajeClub",
                column: "IdUtilizator",
                principalTable: "Users",
                principalColumn: "id_utilizator",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
