using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ICU.Data.Migrations
{
    public partial class AddCpaxAchievement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Achievements",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PatientId = table.Column<long>(nullable: false),
                    Value = table.Column<string>(maxLength: 450, nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Achievements_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CPAXes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PatientId = table.Column<long>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false),
                    Grip = table.Column<int>(nullable: false),
                    Respiratory = table.Column<int>(nullable: false),
                    Cough = table.Column<int>(nullable: false),
                    BedMovement = table.Column<int>(nullable: false),
                    DynamicSitting = table.Column<int>(nullable: false),
                    StandingBalance = table.Column<int>(nullable: false),
                    SitToStand = table.Column<int>(nullable: false),
                    BedToChair = table.Column<int>(nullable: false),
                    Stepping = table.Column<int>(nullable: false),
                    Transfer = table.Column<int>(nullable: false),
                    IsGoal = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CPAXes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CPAXes_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_PatientId",
                table: "Achievements",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_CPAXes_PatientId_IsGoal",
                table: "CPAXes",
                columns: new[] { "PatientId", "IsGoal" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Achievements");

            migrationBuilder.DropTable(
                name: "CPAXes");
        }
    }
}
