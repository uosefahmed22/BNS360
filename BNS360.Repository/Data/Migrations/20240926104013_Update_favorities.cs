using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BNS360.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_favorities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_BusinessModels_businessId",
                table: "Favorites");

            migrationBuilder.AlterColumn<int>(
                name: "businessId",
                table: "Favorites",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CraftsMenId",
                table: "Favorites",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_CraftsMenId",
                table: "Favorites",
                column: "CraftsMenId");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_BusinessModels_businessId",
                table: "Favorites",
                column: "businessId",
                principalTable: "BusinessModels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_CraftsMen_CraftsMenId",
                table: "Favorites",
                column: "CraftsMenId",
                principalTable: "CraftsMen",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_BusinessModels_businessId",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_CraftsMen_CraftsMenId",
                table: "Favorites");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_CraftsMenId",
                table: "Favorites");

            migrationBuilder.DropColumn(
                name: "CraftsMenId",
                table: "Favorites");

            migrationBuilder.AlterColumn<int>(
                name: "businessId",
                table: "Favorites",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_BusinessModels_businessId",
                table: "Favorites",
                column: "businessId",
                principalTable: "BusinessModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
