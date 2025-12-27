using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeInventory.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFinancialProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "current_value_amount",
                table: "inventory_items");

            migrationBuilder.DropColumn(
                name: "current_value_currency",
                table: "inventory_items");

            migrationBuilder.DropColumn(
                name: "purchase_date",
                table: "inventory_items");

            migrationBuilder.DropColumn(
                name: "purchase_price_amount",
                table: "inventory_items");

            migrationBuilder.DropColumn(
                name: "purchase_price_currency",
                table: "inventory_items");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "current_value_amount",
                table: "inventory_items",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "current_value_currency",
                table: "inventory_items",
                type: "character varying(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "purchase_date",
                table: "inventory_items",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "purchase_price_amount",
                table: "inventory_items",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "purchase_price_currency",
                table: "inventory_items",
                type: "character varying(3)",
                maxLength: 3,
                nullable: true);
        }
    }
}
