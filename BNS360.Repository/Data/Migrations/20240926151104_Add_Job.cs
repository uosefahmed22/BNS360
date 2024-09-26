using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BNS360.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Job : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobTitleArabic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobTitleEnglish = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobDescriptionArabic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobDescriptionEnglish = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddreesInArabic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddreesInEnglish = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Numbers = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    WorkHours = table.Column<int>(type: "int", nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Requirements = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeAddedjob = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jobs_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_AppUserId",
                table: "Jobs",
                column: "AppUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Jobs");
        }
    }
}
