using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AplicatieLicenta.Migrations
{
    /// <inheritdoc />
    public partial class StergeTabelePiese : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PieseDeblocate");

            migrationBuilder.DropTable(
                name: "PieseAvatar");

            migrationBuilder.DropTable(
                name: "Avatare");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Avatare",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nivel = table.Column<int>(type: "int", nullable: false),
                    Nume = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avatare", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PieseAvatar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AvatarId = table.Column<int>(type: "int", nullable: false),
                    ImagineUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nume = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PieseAvatar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PieseAvatar_Avatare_AvatarId",
                        column: x => x.AvatarId,
                        principalTable: "Avatare",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PieseDeblocate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PiesaAvatarId = table.Column<int>(type: "int", nullable: false),
                    UtilizatorIdUtilizator = table.Column<int>(type: "int", nullable: false),
                    DataDeblocare = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PieseDeblocate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PieseDeblocate_PieseAvatar_PiesaAvatarId",
                        column: x => x.PiesaAvatarId,
                        principalTable: "PieseAvatar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PieseDeblocate_Users_UtilizatorIdUtilizator",
                        column: x => x.UtilizatorIdUtilizator,
                        principalTable: "Users",
                        principalColumn: "id_utilizator",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PieseAvatar_AvatarId",
                table: "PieseAvatar",
                column: "AvatarId");

            migrationBuilder.CreateIndex(
                name: "IX_PieseDeblocate_PiesaAvatarId",
                table: "PieseDeblocate",
                column: "PiesaAvatarId");

            migrationBuilder.CreateIndex(
                name: "IX_PieseDeblocate_UtilizatorIdUtilizator",
                table: "PieseDeblocate",
                column: "UtilizatorIdUtilizator");
        }
    }
}
