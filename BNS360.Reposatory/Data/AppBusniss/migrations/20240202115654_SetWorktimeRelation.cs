using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BNS360.Reposatory.data.appbusniss.migrations
{
    /// <inheritdoc />
    public partial class SetWorktimeRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkTime_Busnisses_BusnissId",
                table: "WorkTime");

            migrationBuilder.DropTable(
                name: "IdentityRole");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkTime_Busnisses_BusnissId",
                table: "WorkTime",
                column: "BusnissId",
                principalTable: "Busnisses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkTime_Busnisses_BusnissId",
                table: "WorkTime");

            migrationBuilder.CreateTable(
                name: "IdentityRole",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRole", x => x.Id);
                });

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
                name: "FK_WorkTime_Busnisses_BusnissId",
                table: "WorkTime",
                column: "BusnissId",
                principalTable: "Busnisses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
