using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeInventory.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    parent_category_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "inventory_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    purchase_price_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    purchase_price_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    current_value_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    current_value_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    purchase_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    room = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    storage_spot = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    gps_latitude = table.Column<double>(type: "double precision", nullable: true),
                    gps_longitude = table.Column<double>(type: "double precision", nullable: true),
                    category_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory_items", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "item_photos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    file_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    media_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    uploaded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    file_size_bytes = table.Column<long>(type: "bigint", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_photos", x => x.id);
                    table.ForeignKey(
                        name: "FK_item_photos_inventory_items_item_id",
                        column: x => x.item_id,
                        principalTable: "inventory_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "item_receipts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    file_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    media_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    uploaded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    file_size_bytes = table.Column<long>(type: "bigint", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_receipts", x => x.id);
                    table.ForeignKey(
                        name: "FK_item_receipts_inventory_items_item_id",
                        column: x => x.item_id,
                        principalTable: "inventory_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "item_tags",
                columns: table => new
                {
                    tag_value = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "ix_categories_name",
                table: "categories",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_categories_parent_category_id",
                table: "categories",
                column: "parent_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_items_category_id",
                table: "inventory_items",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_items_created_at",
                table: "inventory_items",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_item_photos_item_id",
                table: "item_photos",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_item_receipts_item_id",
                table: "item_receipts",
                column: "item_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "item_photos");

            migrationBuilder.DropTable(
                name: "item_receipts");

            migrationBuilder.DropTable(
                name: "item_tags");

            migrationBuilder.DropTable(
                name: "inventory_items");
        }
    }
}
