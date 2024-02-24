using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BNS360.Reposatory.data.appbusniss.Migrations
{
    /// <inheritdoc />
    public partial class AddEng : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Busnisses",
                newName: "NameAR");

            migrationBuilder.RenameColumn(
                name: "About",
                table: "Busnisses",
                newName: "AboutAR");

            migrationBuilder.AddColumn<string>(
                name: "AboutENG",
                table: "Busnisses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameENG",
                table: "Busnisses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AboutENG",
                table: "Busnisses");

            migrationBuilder.DropColumn(
                name: "NameENG",
                table: "Busnisses");

            migrationBuilder.RenameColumn(
                name: "NameAR",
                table: "Busnisses",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "AboutAR",
                table: "Busnisses",
                newName: "About");
        }
    }
}
