using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AplicatieLicenta.Migrations
{
    /// <inheritdoc />
    public partial class SchimbareTabelaCarti : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "categorie_varsta",
                table: "Carti");

            migrationBuilder.CreateTable(
                name: "CategorieVarsta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Denumire = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategorieVarsta", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Denumire = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarteCategorieVarsta",
                columns: table => new
                {
                    CartiIdCarte = table.Column<int>(type: "int", nullable: false),
                    CategoriiVarstaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarteCategorieVarsta", x => new { x.CartiIdCarte, x.CategoriiVarstaId });
                    table.ForeignKey(
                        name: "FK_CarteCategorieVarsta_Carti_CartiIdCarte",
                        column: x => x.CartiIdCarte,
                        principalTable: "Carti",
                        principalColumn: "id_carte",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarteCategorieVarsta_CategorieVarsta_CategoriiVarstaId",
                        column: x => x.CategoriiVarstaId,
                        principalTable: "CategorieVarsta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarteGen",
                columns: table => new
                {
                    CartiIdCarte = table.Column<int>(type: "int", nullable: false),
                    GenuriId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarteGen", x => new { x.CartiIdCarte, x.GenuriId });
                    table.ForeignKey(
                        name: "FK_CarteGen_Carti_CartiIdCarte",
                        column: x => x.CartiIdCarte,
                        principalTable: "Carti",
                        principalColumn: "id_carte",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarteGen_Gen_GenuriId",
                        column: x => x.GenuriId,
                        principalTable: "Gen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarteCategorieVarsta_CategoriiVarstaId",
                table: "CarteCategorieVarsta",
                column: "CategoriiVarstaId");

            migrationBuilder.CreateIndex(
                name: "IX_CarteGen_GenuriId",
                table: "CarteGen",
                column: "GenuriId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarteCategorieVarsta");

            migrationBuilder.DropTable(
                name: "CarteGen");

            migrationBuilder.DropTable(
                name: "CategorieVarsta");

            migrationBuilder.DropTable(
                name: "Gen");

            migrationBuilder.AddColumn<string>(
                name: "categorie_varsta",
                table: "Carti",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
