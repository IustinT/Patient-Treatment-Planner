using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ICU.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImageCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 450, nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageCategories", x => x.Id);
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
                    Hospital = table.Column<string>(maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
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

            migrationBuilder.CreateIndex(
                name: "IX_Goals_PatientId_IsMainGoal",
                table: "Goals",
                columns: new[] { "PatientId", "IsMainGoal" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Goals");

            migrationBuilder.DropTable(
                name: "ImageCategories");

            migrationBuilder.DropTable(
                name: "Patients");
        }
    }
}
