using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenVids.Data.Access.Migrations
{
    /// <inheritdoc />
    public partial class Commentsandviewsnew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VideoView_AspNetUsers_AppUserId",
                table: "VideoView");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoView_Videos_VideoId",
                table: "VideoView");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VideoView",
                table: "VideoView");

            migrationBuilder.RenameTable(
                name: "VideoView",
                newName: "VideoViews");

            migrationBuilder.RenameIndex(
                name: "IX_VideoView_VideoId",
                table: "VideoViews",
                newName: "IX_VideoViews_VideoId");

            migrationBuilder.RenameIndex(
                name: "IX_VideoView_AppUserId",
                table: "VideoViews",
                newName: "IX_VideoViews_AppUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideoViews",
                table: "VideoViews",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VideoViews_AspNetUsers_AppUserId",
                table: "VideoViews",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoViews_Videos_VideoId",
                table: "VideoViews",
                column: "VideoId",
                principalTable: "Videos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VideoViews_AspNetUsers_AppUserId",
                table: "VideoViews");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoViews_Videos_VideoId",
                table: "VideoViews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VideoViews",
                table: "VideoViews");

            migrationBuilder.RenameTable(
                name: "VideoViews",
                newName: "VideoView");

            migrationBuilder.RenameIndex(
                name: "IX_VideoViews_VideoId",
                table: "VideoView",
                newName: "IX_VideoView_VideoId");

            migrationBuilder.RenameIndex(
                name: "IX_VideoViews_AppUserId",
                table: "VideoView",
                newName: "IX_VideoView_AppUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideoView",
                table: "VideoView",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VideoView_AspNetUsers_AppUserId",
                table: "VideoView",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoView_Videos_VideoId",
                table: "VideoView",
                column: "VideoId",
                principalTable: "Videos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
