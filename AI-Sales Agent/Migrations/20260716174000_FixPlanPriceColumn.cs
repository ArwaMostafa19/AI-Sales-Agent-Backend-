using AI_Sales_Agent.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AI_Sales_Agent.Migrations
{
    /// <summary>
    /// Repairs databases where AddPlanPriceColumn was applied while its Up method was empty.
    /// </summary>
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260716174000_FixPlanPriceColumn")]
    public partial class FixPlanPriceColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'[dbo].[Plans]', N'PlanPrice') IS NULL
BEGIN
    ALTER TABLE [dbo].[Plans]
    ADD [PlanPrice] decimal(18,2) NOT NULL CONSTRAINT [DF_Plans_PlanPrice] DEFAULT 0;
END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'[dbo].[Plans]', N'PlanPrice') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Plans] DROP CONSTRAINT [DF_Plans_PlanPrice];
    ALTER TABLE [dbo].[Plans] DROP COLUMN [PlanPrice];
END");
        }

        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
        }
    }
}
