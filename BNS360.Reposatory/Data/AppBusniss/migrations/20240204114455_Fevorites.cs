using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BNS360.Reposatory.data.appbusniss.migrations
{
    /// <inheritdoc />
    public partial class Fevorites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastModefied",
                table: "Reviews",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FavoriteBusnisses",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusnissId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteBusnisses", x => new { x.UserId, x.BusnissId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriteBusnisses");

            migrationBuilder.DropColumn(
                name: "LastModefied",
                table: "Reviews");
        }
    }
}
