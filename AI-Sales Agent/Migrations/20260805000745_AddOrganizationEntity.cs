using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI_Sales_Agent.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                table: "Subscriptions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                table: "Stores",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                table: "Features",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubscriptionPlan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubscriptionStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timezone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organizations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_OrganizationId",
                table: "Subscriptions",
                column: "OrganizationId",
                unique: true,
                filter: "[OrganizationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_OrganizationId",
                table: "Stores",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Features_OrganizationId",
                table: "Features",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_UserId",
                table: "Organizations",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Features_Organizations_OrganizationId",
                table: "Features",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_Organizations_OrganizationId",
                table: "Stores",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Organizations_OrganizationId",
                table: "Subscriptions",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Features_Organizations_OrganizationId",
                table: "Features");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Organizations_OrganizationId",
                table: "Stores");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Organizations_OrganizationId",
                table: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_OrganizationId",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Stores_OrganizationId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Features_OrganizationId",
                table: "Features");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Features");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "AspNetUsers");
        }
    }
}
