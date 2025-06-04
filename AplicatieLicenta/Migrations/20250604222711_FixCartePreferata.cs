using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AplicatieLicenta.Migrations
{
    /// <inheritdoc />
    public partial class FixCartePreferata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
            @"
            IF OBJECT_ID('CartiPreferate', 'U') IS NOT NULL
            BEGIN
                IF EXISTS (
                    SELECT 1 FROM sys.foreign_keys 
                    WHERE name = 'FK_CartiPreferate_Carti_CarteIdCarte'
                )
                    ALTER TABLE [CartiPreferate] DROP CONSTRAINT [FK_CartiPreferate_Carti_CarteIdCarte];

                IF EXISTS (
                    SELECT 1 FROM sys.foreign_keys 
                    WHERE name = 'FK_CartiPreferate_Users_UtilizatorIdUtilizator'
                )
                    ALTER TABLE [CartiPreferate] DROP CONSTRAINT [FK_CartiPreferate_Users_UtilizatorIdUtilizator];

                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_CartiPreferate_CarteIdCarte')
                    DROP INDEX [IX_CartiPreferate_CarteIdCarte] ON [CartiPreferate];

                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_CartiPreferate_IdUtilizator_IdCarte')
                    DROP INDEX [IX_CartiPreferate_IdUtilizator_IdCarte] ON [CartiPreferate];

                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_CartiPreferate_UtilizatorIdUtilizator')
                    DROP INDEX [IX_CartiPreferate_UtilizatorIdUtilizator] ON [CartiPreferate];

                IF COL_LENGTH('CartiPreferate', 'CarteIdCarte') IS NOT NULL
                    ALTER TABLE [CartiPreferate] DROP COLUMN [CarteIdCarte];

                IF COL_LENGTH('CartiPreferate', 'UtilizatorIdUtilizator') IS NOT NULL
                    ALTER TABLE [CartiPreferate] DROP COLUMN [UtilizatorIdUtilizator];
            END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartiPreferate_Carti_IdCarte",
                table: "CartiPreferate");

            migrationBuilder.DropForeignKey(
                name: "FK_CartiPreferate_Users_IdUtilizator",
                table: "CartiPreferate");

            migrationBuilder.DropIndex(
                name: "IX_CartiPreferate_IdCarte",
                table: "CartiPreferate");

            migrationBuilder.DropIndex(
                name: "IX_CartiPreferate_IdUtilizator",
                table: "CartiPreferate");

            migrationBuilder.AddColumn<int>(
                name: "CarteIdCarte",
                table: "CartiPreferate",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UtilizatorIdUtilizator",
                table: "CartiPreferate",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddForeignKey(
                name: "FK_CartiPreferate_Carti_CarteIdCarte",
                table: "CartiPreferate",
                column: "CarteIdCarte",
                principalTable: "Carti",
                principalColumn: "id_carte",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartiPreferate_Users_UtilizatorIdUtilizator",
                table: "CartiPreferate",
                column: "UtilizatorIdUtilizator",
                principalTable: "Users",
                principalColumn: "id_utilizator",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
