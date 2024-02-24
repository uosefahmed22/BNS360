using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BNS360.Reposatory.data.identity.Migrations
{
    /// <inheritdoc />
    public partial class AddPiectureFeild : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "401123cf-bd80-407d-b8cb-65a66c1cbc1b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "73f6b9a0-a4cf-4bc9-a95e-a7746aeee492");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9ebb051c-64b3-419f-8dbc-1c31910eae8a");

            migrationBuilder.AddColumn<string>(
                name: "profilePictureUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1d450859-5556-4f59-8c4b-4a5337b0ba45", "b9d51b05-0c9d-407c-98c6-80cb9dd69833", "ServiceProvider", "SERVICEPROVIDER" },
                    { "1ee306e5-fdec-4771-9ad3-86d5f076d0b7", "397b115b-c179-4ebe-a188-f662d025ac6e", "Default", "DEFAULT" },
                    { "c812f03d-ba31-4e6f-894f-79b92c8a2074", "17a2e694-966b-47a6-a83a-1391fe7e2592", "BusinssOwner", "BUSINSSOWNER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1d450859-5556-4f59-8c4b-4a5337b0ba45");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1ee306e5-fdec-4771-9ad3-86d5f076d0b7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c812f03d-ba31-4e6f-894f-79b92c8a2074");

            migrationBuilder.DropColumn(
                name: "profilePictureUrl",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusnissId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rate = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "401123cf-bd80-407d-b8cb-65a66c1cbc1b", "b2630af7-1b08-489a-aee9-a237469f9034", "ServiceProvider", "SERVICEPROVIDER" },
                    { "73f6b9a0-a4cf-4bc9-a95e-a7746aeee492", "14e69019-a57b-403d-a572-31e74ddb188a", "Default", "DEFAULT" },
                    { "9ebb051c-64b3-419f-8dbc-1c31910eae8a", "e81bc523-de81-452e-b0d6-75d526fb2998", "BusinssOwner", "BUSINSSOWNER" }
                });
        }
    }
}
