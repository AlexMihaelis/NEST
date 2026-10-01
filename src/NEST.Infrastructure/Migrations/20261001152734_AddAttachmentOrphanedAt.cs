using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NEST.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAttachmentOrphanedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OrphanedAt",
                table: "Attachments",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrphanedAt",
                table: "Attachments");
        }
    }
}
