using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class _02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GroupName",
                table: "WorkToolLog",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "GroupRemark",
                table: "WorkToolLog",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ReceivedName",
                table: "WorkToolLog",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<short>(
                name: "Type",
                table: "WorkToolLog",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupName",
                table: "WorkToolLog");

            migrationBuilder.DropColumn(
                name: "GroupRemark",
                table: "WorkToolLog");

            migrationBuilder.DropColumn(
                name: "ReceivedName",
                table: "WorkToolLog");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "WorkToolLog");
        }
    }
}
