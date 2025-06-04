using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AplicatieLicenta.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCartePreferata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("IF OBJECT_ID('CartiPreferate', 'U') IS NOT NULL DROP TABLE CartiPreferate;");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CartiPreferate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCarte = table.Column<int>(type: "int", nullable: false),
                    IdUtilizator = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartiPreferate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartiPreferate_Carti_IdCarte",
                        column: x => x.IdCarte,
                        principalTable: "Carti",
                        principalColumn: "id_carte",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartiPreferate_Users_IdUtilizator",
                        column: x => x.IdUtilizator,
                        principalTable: "Users",
                        principalColumn: "id_utilizator",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartiPreferate_IdCarte",
                table: "CartiPreferate",
                column: "IdCarte");

            migrationBuilder.CreateIndex(
                name: "IX_CartiPreferate_IdUtilizator",
                table: "CartiPreferate",
                column: "IdUtilizator");
        }
    }
}
