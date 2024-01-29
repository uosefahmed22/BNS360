using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BNS360.Reposatory.data.appbusniss.migrations
{
    /// <inheritdoc />
    public partial class addUserIdToReviewTabel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Review_Busnisses_BusnissId",
                table: "Review");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Review",
                table: "Review");

            migrationBuilder.DeleteData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "4a13a68f-a8bb-4481-a652-20f34156bdea");

            migrationBuilder.DeleteData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "68f2747d-b2f2-4624-8423-a19039ae1c84");

            migrationBuilder.DeleteData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "7ab5458c-f3c2-4744-9813-989a0085a3c5");

            migrationBuilder.RenameTable(
                name: "Review",
                newName: "Reviews");

            migrationBuilder.RenameIndex(
                name: "IX_Review_BusnissId",
                table: "Reviews",
                newName: "IX_Reviews_BusnissId");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Reviews",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reviews",
                table: "Reviews",
                column: "Id");

            migrationBuilder.InsertData(
                table: "IdentityRole",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2b414bc4-e7f4-417b-9fb7-899f28f6caec", "35bebd76-1e74-499e-a242-8a56cce8262a", "Default", "DEFAULT" },
                    { "31940441-c040-43b7-9551-5ad4a033f9d7", "80d84e4e-de5c-413f-bf10-776b4d59eef0", "ServiceProvider", "SERVICEPROVIDER" },
                    { "8a6290af-e192-4def-b22d-bcc63ad0855e", "87b3a328-b0b0-4037-af16-d66f6f3d2338", "BusinssOwner", "BUSINSSOWNER" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Busnisses_BusnissId",
                table: "Reviews",
                column: "BusnissId",
                principalTable: "Busnisses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Busnisses_BusnissId",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reviews",
                table: "Reviews");

            migrationBuilder.DeleteData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "2b414bc4-e7f4-417b-9fb7-899f28f6caec");

            migrationBuilder.DeleteData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "31940441-c040-43b7-9551-5ad4a033f9d7");

            migrationBuilder.DeleteData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "8a6290af-e192-4def-b22d-bcc63ad0855e");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Reviews");

            migrationBuilder.RenameTable(
                name: "Reviews",
                newName: "Review");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_BusnissId",
                table: "Review",
                newName: "IX_Review_BusnissId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Review",
                table: "Review",
                column: "Id");

            migrationBuilder.InsertData(
                table: "IdentityRole",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4a13a68f-a8bb-4481-a652-20f34156bdea", "60cf3337-9e50-4cf6-a4a6-ddd8aebd0a85", "BusinssOwner", "BUSINSSOWNER" },
                    { "68f2747d-b2f2-4624-8423-a19039ae1c84", "e6e306ee-2ef4-43d7-a61e-6900b9732a2a", "Default", "DEFAULT" },
                    { "7ab5458c-f3c2-4744-9813-989a0085a3c5", "cafc8012-e2b5-447a-bddf-0a014848ffd1", "ServiceProvider", "SERVICEPROVIDER" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Busnisses_BusnissId",
                table: "Review",
                column: "BusnissId",
                principalTable: "Busnisses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
