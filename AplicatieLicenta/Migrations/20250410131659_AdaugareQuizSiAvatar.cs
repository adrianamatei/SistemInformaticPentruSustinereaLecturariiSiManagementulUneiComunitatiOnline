using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AplicatieLicenta.Migrations
{
    /// <inheritdoc />
    public partial class AdaugareQuizSiAvatar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Avatare",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nivel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avatare", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Quizuri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarteId = table.Column<int>(type: "int", nullable: false),
                    Titlu = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizuri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quizuri_Carti_CarteId",
                        column: x => x.CarteId,
                        principalTable: "Carti",
                        principalColumn: "id_carte",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PieseAvatar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AvatarId = table.Column<int>(type: "int", nullable: false),
                    Nume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagineUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                name: "IntrebariQuiz",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuizId = table.Column<int>(type: "int", nullable: false),
                    Enunt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Categorie = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntrebariQuiz", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntrebariQuiz_Quizuri_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizuri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RezultateQuiz",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuizId = table.Column<int>(type: "int", nullable: false),
                    Scor = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UtilizatorIdUtilizator = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RezultateQuiz", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RezultateQuiz_Quizuri_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizuri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RezultateQuiz_Users_UtilizatorIdUtilizator",
                        column: x => x.UtilizatorIdUtilizator,
                        principalTable: "Users",
                        principalColumn: "id_utilizator",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PieseDeblocate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PiesaAvatarId = table.Column<int>(type: "int", nullable: false),
                    DataDeblocare = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UtilizatorIdUtilizator = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "VarianteRaspuns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IntrebareQuizId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EsteCorect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VarianteRaspuns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VarianteRaspuns_IntrebariQuiz_IntrebareQuizId",
                        column: x => x.IntrebareQuizId,
                        principalTable: "IntrebariQuiz",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IntrebariQuiz_QuizId",
                table: "IntrebariQuiz",
                column: "QuizId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Quizuri_CarteId",
                table: "Quizuri",
                column: "CarteId");

            migrationBuilder.CreateIndex(
                name: "IX_RezultateQuiz_QuizId",
                table: "RezultateQuiz",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_RezultateQuiz_UtilizatorIdUtilizator",
                table: "RezultateQuiz",
                column: "UtilizatorIdUtilizator");

            migrationBuilder.CreateIndex(
                name: "IX_VarianteRaspuns_IntrebareQuizId",
                table: "VarianteRaspuns",
                column: "IntrebareQuizId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PieseDeblocate");

            migrationBuilder.DropTable(
                name: "RezultateQuiz");

            migrationBuilder.DropTable(
                name: "VarianteRaspuns");

            migrationBuilder.DropTable(
                name: "PieseAvatar");

            migrationBuilder.DropTable(
                name: "IntrebariQuiz");

            migrationBuilder.DropTable(
                name: "Avatare");

            migrationBuilder.DropTable(
                name: "Quizuri");
        }
    }
}
