using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GooseShop.Migrations
{
    /// <inheritdoc />
    public partial class AddWarehouseIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CachedWarehouses_CityRef",
                table: "CachedWarehouses",
                column: "CityRef");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CachedWarehouses_CityRef",
                table: "CachedWarehouses");
        }
    }
}
