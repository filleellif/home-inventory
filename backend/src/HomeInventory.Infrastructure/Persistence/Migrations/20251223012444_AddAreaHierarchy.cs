using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeInventory.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAreaHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "parent_area_id",
                table: "areas",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_areas_parent_area_id",
                table: "areas",
                column: "parent_area_id");

            migrationBuilder.AddForeignKey(
                name: "FK_areas_areas_parent_area_id",
                table: "areas",
                column: "parent_area_id",
                principalTable: "areas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_areas_areas_parent_area_id",
                table: "areas");

            migrationBuilder.DropIndex(
                name: "ix_areas_parent_area_id",
                table: "areas");

            migrationBuilder.DropColumn(
                name: "parent_area_id",
                table: "areas");
        }
    }
}
