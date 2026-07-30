using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI_Sales_Agent.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueShopDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                WITH DuplicateStores AS (
                    SELECT Id,
                           ROW_NUMBER() OVER (PARTITION BY LOWER(TRIM(ShopDomain)) ORDER BY CreatedAt ASC) as RowNum
                    FROM Stores
                    WHERE DeletedAt IS NULL
                )
                UPDATE Stores
                SET DeletedAt = GETUTCDATE(),
                    Status = 'Deleted (Duplicate Cleanup)'
                WHERE Id IN (
                    SELECT Id
                    FROM DuplicateStores
                    WHERE RowNum > 1
                );
            ");

            migrationBuilder.AlterColumn<string>(
                name: "ShopDomain",
                table: "Stores",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_ShopDomain",
                table: "Stores",
                column: "ShopDomain",
                unique: true,
                filter: "[DeletedAt] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stores_ShopDomain",
                table: "Stores");

            migrationBuilder.AlterColumn<string>(
                name: "ShopDomain",
                table: "Stores",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);
        }
    }
}
