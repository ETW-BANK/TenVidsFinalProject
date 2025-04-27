using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenVids.Data.Access.Migrations
{
    /// <inheritdoc />
    public partial class comments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Comment",
                newName: "PostedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PostedAt",
                table: "Comment",
                newName: "CreatedAt");
        }
    }
}
