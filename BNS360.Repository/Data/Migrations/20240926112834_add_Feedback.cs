using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BNS360.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class add_Feedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Feedback = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    rating = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BusinessModelId = table.Column<int>(type: "int", nullable: true),
                    CraftsMenModelId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feedbacks_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Feedbacks_BusinessModels_BusinessModelId",
                        column: x => x.BusinessModelId,
                        principalTable: "BusinessModels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Feedbacks_CraftsMen_CraftsMenModelId",
                        column: x => x.CraftsMenModelId,
                        principalTable: "CraftsMen",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_BusinessModelId",
                table: "Feedbacks",
                column: "BusinessModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_CraftsMenModelId",
                table: "Feedbacks",
                column: "CraftsMenModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_UserId",
                table: "Feedbacks",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Feedbacks");
        }
    }
}
