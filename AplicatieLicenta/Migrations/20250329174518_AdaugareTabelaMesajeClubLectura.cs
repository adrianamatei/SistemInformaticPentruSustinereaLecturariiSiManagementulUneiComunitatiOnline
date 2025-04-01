using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AplicatieLicenta.Migrations
{
    /// <inheritdoc />
    public partial class AdaugareTabelaMesajeClubLectura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "MembriClub",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "MesajeClub",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdClub = table.Column<int>(type: "int", nullable: false),
                    IdUtilizator = table.Column<int>(type: "int", nullable: false),
                    Continut = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataTrimiterii = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MesajeClub", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MesajeClub_CluburiLectura_IdClub",
                        column: x => x.IdClub,
                        principalTable: "CluburiLectura",
                        principalColumn: "id_club",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MesajeClub_Users_IdUtilizator",
                        column: x => x.IdUtilizator,
                        principalTable: "Users",
                        principalColumn: "id_utilizator",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MesajeClub_IdClub",
                table: "MesajeClub",
                column: "IdClub");

            migrationBuilder.CreateIndex(
                name: "IX_MesajeClub_IdUtilizator",
                table: "MesajeClub",
                column: "IdUtilizator");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MesajeClub");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "MembriClub");
        }
    }
}
