using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BNS360.Reposatory.data.appbusniss.migrations
{
    /// <inheritdoc />
    public partial class AddAlbumUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AlbumUrl",
                table: "Busnisses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlbumUrl",
                table: "Busnisses");
        }
    }
}
