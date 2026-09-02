using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduationProject.Migrations
{
    /// <inheritdoc />
    public partial class AddPredictionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiabetesPredictions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Hypertension = table.Column<bool>(type: "bit", nullable: false),
                    HeartDisease = table.Column<bool>(type: "bit", nullable: false),
                    SmokingHistory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BMI = table.Column<float>(type: "real", nullable: false),
                    HbA1cLevel = table.Column<float>(type: "real", nullable: false),
                    BloodGlucoseLevel = table.Column<float>(type: "real", nullable: false),
                    PredictionResult = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiabetesPredictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiabetesPredictions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiabetesPredictions_UserId",
                table: "DiabetesPredictions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiabetesPredictions");
        }
    }
}
