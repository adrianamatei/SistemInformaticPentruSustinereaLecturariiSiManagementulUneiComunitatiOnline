using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AplicatieLicenta.Migrations
{
    /// <inheritdoc />
    public partial class AdaugaTabelaCartiPreferate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CartiPreferate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUtilizator = table.Column<int>(type: "int", nullable: false),
                    UtilizatorIdUtilizator = table.Column<int>(type: "int", nullable: false),
                    IdCarte = table.Column<int>(type: "int", nullable: false),
                    CarteIdCarte = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartiPreferate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartiPreferate_Carti_CarteIdCarte",
                        column: x => x.CarteIdCarte,
                        principalTable: "Carti",
                        principalColumn: "id_carte",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartiPreferate_Users_UtilizatorIdUtilizator",
                        column: x => x.UtilizatorIdUtilizator,
                        principalTable: "Users",
                        principalColumn: "id_utilizator",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartiPreferate_CarteIdCarte",
                table: "CartiPreferate",
                column: "CarteIdCarte");

            migrationBuilder.CreateIndex(
                name: "IX_CartiPreferate_IdUtilizator_IdCarte",
                table: "CartiPreferate",
                columns: new[] { "IdUtilizator", "IdCarte" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartiPreferate_UtilizatorIdUtilizator",
                table: "CartiPreferate",
                column: "UtilizatorIdUtilizator");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartiPreferate");
        }
    }
}
