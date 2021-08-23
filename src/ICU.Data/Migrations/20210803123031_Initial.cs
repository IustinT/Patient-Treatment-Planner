using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ICU.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BaseCategory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 450, nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    CategoryType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhoneNumber = table.Column<string>(maxLength: 50, nullable: false),
                    Name = table.Column<string>(maxLength: 450, nullable: false),
                    AdmissionDate = table.Column<DateTime>(nullable: false),
                    Ward = table.Column<string>(maxLength: 450, nullable: false),
                    Hospital = table.Column<string>(maxLength: 450, nullable: false),
                    MondayExerciseTime = table.Column<int>(nullable: true),
                    TuesdayExerciseTime = table.Column<int>(nullable: true),
                    WednesdayExerciseTime = table.Column<int>(nullable: true),
                    ThursdayExerciseTime = table.Column<int>(nullable: true),
                    FridayExerciseTime = table.Column<int>(nullable: true),
                    SaturdayExerciseTime = table.Column<int>(nullable: true),
                    SunExerciseTime = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 450, nullable: false),
                    CategoryId = table.Column<int>(nullable: false),
                    Aim = table.Column<string>(nullable: true),
                    Instructions = table.Column<string>(nullable: true),
                    Variations = table.Column<string>(nullable: true),
                    Precautions = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exercises_BaseCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "BaseCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "Goals",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PatientId = table.Column<long>(nullable: false),
                    Value = table.Column<string>(maxLength: 450, nullable: false),
                    IsMainGoal = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Goals_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientExercises",
                columns: table => new
                {
                    PatientId = table.Column<long>(nullable: false),
                    ExerciseId = table.Column<long>(nullable: false),
                    Repetitions = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientExercises", x => new { x.ExerciseId, x.PatientId });
                    table.ForeignKey(
                        name: "FK_PatientExercises_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientExercises_Patients_PatientId",
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

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_CategoryId",
                table: "Exercises",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_PatientId_IsMainGoal",
                table: "Goals",
                columns: new[] { "PatientId", "IsMainGoal" });

            migrationBuilder.CreateIndex(
                name: "IX_PatientExercises_PatientId_ExerciseId",
                table: "PatientExercises",
                columns: new[] { "PatientId", "ExerciseId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Achievements");

            migrationBuilder.DropTable(
                name: "CPAXes");

            migrationBuilder.DropTable(
                name: "Goals");

            migrationBuilder.DropTable(
                name: "PatientExercises");

            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "BaseCategory");
        }
    }
}
