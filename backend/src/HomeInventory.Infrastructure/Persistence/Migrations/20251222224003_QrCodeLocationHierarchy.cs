using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeInventory.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class QrCodeLocationHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "item_tags");

            migrationBuilder.DropColumn(
                name: "gps_latitude",
                table: "inventory_items");

            migrationBuilder.DropColumn(
                name: "gps_longitude",
                table: "inventory_items");

            migrationBuilder.DropColumn(
                name: "room",
                table: "inventory_items");

            migrationBuilder.DropColumn(
                name: "storage_spot",
                table: "inventory_items");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "room",
                table: "inventory_items",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "storage_spot",
                table: "inventory_items",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "gps_latitude",
                table: "inventory_items",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "gps_longitude",
                table: "inventory_items",
                type: "double precision",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "item_tags",
                columns: table => new
                {
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tag_value = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_tags", x => new { x.item_id, x.tag_value });
                    table.ForeignKey(
                        name: "FK_item_tags_inventory_items_item_id",
                        column: x => x.item_id,
                        principalTable: "inventory_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
