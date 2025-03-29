using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AplicatieLicenta.Migrations
{
    /// <inheritdoc />
    public partial class AdaugareTabeleLegatura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarteCategorieVarsta_CategorieVarsta_CategoriiVarstaId",
                table: "CarteCategorieVarsta");

            migrationBuilder.DropForeignKey(
                name: "FK_CarteGen_Gen_GenuriId",
                table: "CarteGen");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Gen",
                table: "Gen");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategorieVarsta",
                table: "CategorieVarsta");

            migrationBuilder.RenameTable(
                name: "Gen",
                newName: "Genuri");

            migrationBuilder.RenameTable(
                name: "CategorieVarsta",
                newName: "CategoriiVarsta");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Genuri",
                table: "Genuri",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoriiVarsta",
                table: "CategoriiVarsta",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CarteCategorieVarsta_CategoriiVarsta_CategoriiVarstaId",
                table: "CarteCategorieVarsta",
                column: "CategoriiVarstaId",
                principalTable: "CategoriiVarsta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CarteGen_Genuri_GenuriId",
                table: "CarteGen",
                column: "GenuriId",
                principalTable: "Genuri",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarteCategorieVarsta_CategoriiVarsta_CategoriiVarstaId",
                table: "CarteCategorieVarsta");

            migrationBuilder.DropForeignKey(
                name: "FK_CarteGen_Genuri_GenuriId",
                table: "CarteGen");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Genuri",
                table: "Genuri");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoriiVarsta",
                table: "CategoriiVarsta");

            migrationBuilder.RenameTable(
                name: "Genuri",
                newName: "Gen");

            migrationBuilder.RenameTable(
                name: "CategoriiVarsta",
                newName: "CategorieVarsta");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Gen",
                table: "Gen",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategorieVarsta",
                table: "CategorieVarsta",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CarteCategorieVarsta_CategorieVarsta_CategoriiVarstaId",
                table: "CarteCategorieVarsta",
                column: "CategoriiVarstaId",
                principalTable: "CategorieVarsta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CarteGen_Gen_GenuriId",
                table: "CarteGen",
                column: "GenuriId",
                principalTable: "Gen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
