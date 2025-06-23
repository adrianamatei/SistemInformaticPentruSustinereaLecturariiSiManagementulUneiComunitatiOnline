using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AplicatieLicenta.Migrations
{
    /// <inheritdoc />
    public partial class StergereVizitatori : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vizitatori");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vizitatori",
                columns: table => new
                {
                    id_vizita = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_utilizator = table.Column<int>(type: "int", nullable: true),
                    data_vizitei = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    tip_utilizator = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Vizitato__8E2EF95ABAC6D5B7", x => x.id_vizita);
                    table.ForeignKey(
                        name: "FK__Vizitator__id_ut__440B1D61",
                        column: x => x.id_utilizator,
                        principalTable: "Users",
                        principalColumn: "id_utilizator");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vizitatori_id_utilizator",
                table: "Vizitatori",
                column: "id_utilizator");
        }
    }
}
