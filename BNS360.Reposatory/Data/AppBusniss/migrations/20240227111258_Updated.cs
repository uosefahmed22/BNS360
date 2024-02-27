using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BNS360.Reposatory.data.appbusniss.Migrations
{
    /// <inheritdoc />
    public partial class Updated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contact_Busnisses_BusnissId",
                table: "Contact");

            migrationBuilder.DropForeignKey(
                name: "FK_Location_Busnisses_BusnissId",
                table: "Location");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Busnisses_BusnissId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_BusnissId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Location_BusnissId",
                table: "Location");

            migrationBuilder.DropIndex(
                name: "IX_Contact_BusnissId",
                table: "Contact");

            migrationBuilder.AddColumn<Guid>(
                name: "BusinessId",
                table: "Reviews",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ContactInfoId",
                table: "Busnisses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "Busnisses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BusinessId",
                table: "Reviews",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Busnisses_ContactInfoId",
                table: "Busnisses",
                column: "ContactInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_Busnisses_LocationId",
                table: "Busnisses",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Busnisses_Contact_ContactInfoId",
                table: "Busnisses",
                column: "ContactInfoId",
                principalTable: "Contact",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Busnisses_Location_LocationId",
                table: "Busnisses",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Busnisses_BusinessId",
                table: "Reviews",
                column: "BusinessId",
                principalTable: "Busnisses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Busnisses_Contact_ContactInfoId",
                table: "Busnisses");

            migrationBuilder.DropForeignKey(
                name: "FK_Busnisses_Location_LocationId",
                table: "Busnisses");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Busnisses_BusinessId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_BusinessId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Busnisses_ContactInfoId",
                table: "Busnisses");

            migrationBuilder.DropIndex(
                name: "IX_Busnisses_LocationId",
                table: "Busnisses");

            migrationBuilder.DropColumn(
                name: "BusinessId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ContactInfoId",
                table: "Busnisses");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Busnisses");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BusnissId",
                table: "Reviews",
                column: "BusnissId");

            migrationBuilder.CreateIndex(
                name: "IX_Location_BusnissId",
                table: "Location",
                column: "BusnissId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contact_BusnissId",
                table: "Contact",
                column: "BusnissId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Contact_Busnisses_BusnissId",
                table: "Contact",
                column: "BusnissId",
                principalTable: "Busnisses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Location_Busnisses_BusnissId",
                table: "Location",
                column: "BusnissId",
                principalTable: "Busnisses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Busnisses_BusnissId",
                table: "Reviews",
                column: "BusnissId",
                principalTable: "Busnisses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
