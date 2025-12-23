using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeInventory.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateArea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_inventory_items_box_qr_code",
                table: "inventory_items");

            migrationBuilder.DropIndex(
                name: "ix_inventory_items_room_qr_code",
                table: "inventory_items");

            migrationBuilder.DropIndex(
                name: "ix_inventory_items_shelf_qr_code",
                table: "inventory_items");

            migrationBuilder.DropColumn(
                name: "box_name",
                table: "inventory_items");

            migrationBuilder.DropColumn(
                name: "box_qr_code",
                table: "inventory_items");

            migrationBuilder.DropColumn(
                name: "room_name",
                table: "inventory_items");

            migrationBuilder.DropColumn(
                name: "room_qr_code",
                table: "inventory_items");

            migrationBuilder.DropColumn(
                name: "shelf_name",
                table: "inventory_items");

            migrationBuilder.DropColumn(
                name: "shelf_qr_code",
                table: "inventory_items");

            migrationBuilder.AddColumn<Guid>(
                name: "area_id",
                table: "inventory_items",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_inventory_items_area_id",
                table: "inventory_items",
                column: "area_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_inventory_items_area_id",
                table: "inventory_items");

            migrationBuilder.DropColumn(
                name: "area_id",
                table: "inventory_items");

            migrationBuilder.AddColumn<string>(
                name: "box_name",
                table: "inventory_items",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "box_qr_code",
                table: "inventory_items",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "room_name",
                table: "inventory_items",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "room_qr_code",
                table: "inventory_items",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "shelf_name",
                table: "inventory_items",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "shelf_qr_code",
                table: "inventory_items",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_inventory_items_box_qr_code",
                table: "inventory_items",
                column: "box_qr_code",
                filter: "box_qr_code IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_items_room_qr_code",
                table: "inventory_items",
                column: "room_qr_code",
                filter: "room_qr_code IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_items_shelf_qr_code",
                table: "inventory_items",
                column: "shelf_qr_code",
                filter: "shelf_qr_code IS NOT NULL");
        }
    }
}
