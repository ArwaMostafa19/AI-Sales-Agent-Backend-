using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI_Sales_Agent.Migrations
{
    /// <inheritdoc />
    public partial class addFreeTrialInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTrial",
                table: "Subscriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "TrialEndDate",
                table: "Subscriptions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TrialStartDate",
                table: "Subscriptions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrialDays",
                table: "Plans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasUsedTrial",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTrial",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "TrialEndDate",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "TrialStartDate",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "TrialDays",
                table: "Plans");

            migrationBuilder.DropColumn(
                name: "HasUsedTrial",
                table: "AspNetUsers");
        }
    }
}
