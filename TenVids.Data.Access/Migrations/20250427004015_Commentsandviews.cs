using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenVids.Data.Access.Migrations
{
    /// <inheritdoc />
    public partial class Commentsandviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VideoViews_AspNetUsers_AppUserId",
                table: "VideoViews");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoViews_Videos_VideoId",
                table: "VideoViews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

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

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Comment",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "Channels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "VideoView",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "VideoView",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "VideoView",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "VideoView",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Is_Proxy",
                table: "VideoView",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastVisit",
                table: "VideoView",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "NumberOfVisits",
                table: "VideoView",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PostCode",
                table: "VideoView",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideoView",
                table: "VideoView",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_AppUserId",
                table: "Comment",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoView_AppUserId",
                table: "VideoView",
                column: "AppUserId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VideoView_AspNetUsers_AppUserId",
                table: "VideoView");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoView_Videos_VideoId",
                table: "VideoView");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_AppUserId",
                table: "Comment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VideoView",
                table: "VideoView");

            migrationBuilder.DropIndex(
                name: "IX_VideoView_AppUserId",
                table: "VideoView");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "VideoView");

            migrationBuilder.DropColumn(
                name: "City",
                table: "VideoView");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "VideoView");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "VideoView");

            migrationBuilder.DropColumn(
                name: "Is_Proxy",
                table: "VideoView");

            migrationBuilder.DropColumn(
                name: "LastVisit",
                table: "VideoView");

            migrationBuilder.DropColumn(
                name: "NumberOfVisits",
                table: "VideoView");

            migrationBuilder.DropColumn(
                name: "PostCode",
                table: "VideoView");

            migrationBuilder.RenameTable(
                name: "VideoView",
                newName: "VideoViews");

            migrationBuilder.RenameIndex(
                name: "IX_VideoView_VideoId",
                table: "VideoViews",
                newName: "IX_VideoViews_VideoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                columns: new[] { "AppUserId", "VideoId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideoViews",
                table: "VideoViews",
                columns: new[] { "AppUserId", "VideoId" });

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
    }
}
