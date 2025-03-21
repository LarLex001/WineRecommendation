using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WineRecommendation.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PredictionResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FixedAcidity = table.Column<float>(type: "REAL", nullable: false),
                    VolatileAcidity = table.Column<float>(type: "REAL", nullable: false),
                    CitricAcid = table.Column<float>(type: "REAL", nullable: false),
                    ResidualSugar = table.Column<float>(type: "REAL", nullable: false),
                    Chlorides = table.Column<float>(type: "REAL", nullable: false),
                    FreeSulfurDioxide = table.Column<float>(type: "REAL", nullable: false),
                    TotalSulfurDioxide = table.Column<float>(type: "REAL", nullable: false),
                    Density = table.Column<float>(type: "REAL", nullable: false),
                    PH = table.Column<float>(type: "REAL", nullable: false),
                    Sulphates = table.Column<float>(type: "REAL", nullable: false),
                    Alcohol = table.Column<float>(type: "REAL", nullable: false),
                    PredictedType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PredictedQuality = table.Column<float>(type: "REAL", nullable: false),
                    PredictionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ContributedToRetraining = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PredictionResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    FixedAcidity = table.Column<float>(type: "REAL", nullable: false),
                    VolatileAcidity = table.Column<float>(type: "REAL", nullable: false),
                    CitricAcid = table.Column<float>(type: "REAL", nullable: false),
                    ResidualSugar = table.Column<float>(type: "REAL", nullable: false),
                    Chlorides = table.Column<float>(type: "REAL", nullable: false),
                    FreeSulfurDioxide = table.Column<float>(type: "REAL", nullable: false),
                    TotalSulfurDioxide = table.Column<float>(type: "REAL", nullable: false),
                    Density = table.Column<float>(type: "REAL", nullable: false),
                    PH = table.Column<float>(type: "REAL", nullable: false),
                    Sulphates = table.Column<float>(type: "REAL", nullable: false),
                    Alcohol = table.Column<float>(type: "REAL", nullable: false),
                    Quality = table.Column<float>(type: "REAL", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsTrainingData = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wines", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PredictionResults");

            migrationBuilder.DropTable(
                name: "Wines");
        }
    }
}
