using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BNS360.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class SecurityHardening : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Existing refresh tokens were stored in plain text. Invalidate them instead of
            // copying sensitive values into the new hashed column.
            migrationBuilder.Sql("DELETE FROM [RefreshTokens]");

            // Keep one row for each logical favorite/saved-job pair before adding unique indexes.
            migrationBuilder.Sql("""
                WITH DuplicateSavedJobs AS
                (
                    SELECT [Id], ROW_NUMBER() OVER (PARTITION BY [UserId], [JobId] ORDER BY [Id]) AS [RowNumber]
                    FROM [SavedJobs]
                )
                DELETE FROM DuplicateSavedJobs WHERE [RowNumber] > 1;
                """);

            migrationBuilder.Sql("""
                WITH DuplicateBusinessFavorites AS
                (
                    SELECT [Id], ROW_NUMBER() OVER (PARTITION BY [UserId], [businessId] ORDER BY [Id]) AS [RowNumber]
                    FROM [Favorites]
                    WHERE [businessId] IS NOT NULL
                )
                DELETE FROM DuplicateBusinessFavorites WHERE [RowNumber] > 1;

                WITH DuplicateCraftsMenFavorites AS
                (
                    SELECT [Id], ROW_NUMBER() OVER (PARTITION BY [UserId], [CraftsMenId] ORDER BY [Id]) AS [RowNumber]
                    FROM [Favorites]
                    WHERE [CraftsMenId] IS NOT NULL
                )
                DELETE FROM DuplicateCraftsMenFavorites WHERE [RowNumber] > 1;

                DELETE FROM [Favorites]
                WHERE ([businessId] IS NULL AND [CraftsMenId] IS NULL)
                   OR ([businessId] IS NOT NULL AND [CraftsMenId] IS NOT NULL);
                """);

            migrationBuilder.Sql("""
                DELETE FROM [Feedbacks]
                WHERE ([BusinessModelId] IS NULL AND [CraftsMenModelId] IS NULL)
                   OR ([BusinessModelId] IS NOT NULL AND [CraftsMenModelId] IS NOT NULL)
                   OR [rating] NOT BETWEEN 1 AND 5;
                """);

            migrationBuilder.DropIndex(
                name: "IX_SavedJobs_UserId",
                table: "SavedJobs");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_UserId",
                table: "Favorites");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "RefreshTokens");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "RefreshTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "JwtId",
                table: "RefreshTokens",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "TokenHash",
                table: "RefreshTokens",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Feedback",
                table: "Feedbacks",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_SavedJobs_UserId_JobId",
                table: "SavedJobs",
                columns: new[] { "UserId", "JobId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_JwtId",
                table: "RefreshTokens",
                column: "JwtId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_TokenHash",
                table: "RefreshTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Feedbacks_ExactlyOneTarget",
                table: "Feedbacks",
                sql: "([BusinessModelId] IS NOT NULL AND [CraftsMenModelId] IS NULL) OR ([BusinessModelId] IS NULL AND [CraftsMenModelId] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Feedbacks_Rating",
                table: "Feedbacks",
                sql: "[rating] BETWEEN 1 AND 5");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId_businessId",
                table: "Favorites",
                columns: new[] { "UserId", "businessId" },
                unique: true,
                filter: "[businessId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId_CraftsMenId",
                table: "Favorites",
                columns: new[] { "UserId", "CraftsMenId" },
                unique: true,
                filter: "[CraftsMenId] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Favorites_ExactlyOneTarget",
                table: "Favorites",
                sql: "([businessId] IS NOT NULL AND [CraftsMenId] IS NULL) OR ([businessId] IS NULL AND [CraftsMenId] IS NOT NULL)");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_AspNetUsers_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_AspNetUsers_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_SavedJobs_UserId_JobId",
                table: "SavedJobs");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_JwtId",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_TokenHash",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Feedbacks_ExactlyOneTarget",
                table: "Feedbacks");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Feedbacks_Rating",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_UserId_businessId",
                table: "Favorites");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_UserId_CraftsMenId",
                table: "Favorites");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Favorites_ExactlyOneTarget",
                table: "Favorites");

            migrationBuilder.DropColumn(
                name: "TokenHash",
                table: "RefreshTokens");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "RefreshTokens",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "JwtId",
                table: "RefreshTokens",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "RefreshTokens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Feedback",
                table: "Feedbacks",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.CreateIndex(
                name: "IX_SavedJobs_UserId",
                table: "SavedJobs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId",
                table: "Favorites",
                column: "UserId");
        }
    }
}
