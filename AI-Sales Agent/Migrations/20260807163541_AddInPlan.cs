using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI_Sales_Agent.Migrations
{
    /// <inheritdoc />
    public partial class AddInPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DevelopmentPrice",
                table: "Plans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DevelopmentPrice",
                table: "Plans");
        }
    }
}
