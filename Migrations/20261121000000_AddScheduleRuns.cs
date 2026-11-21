using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NominationsApi.Migrations;

/// <inheritdoc />
public partial class AddScheduleRuns : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ScheduleRuns",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                PipelineSegment = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                RanAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                Capacity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                TotalRequested = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                Outcome = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ScheduleRuns", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ScheduleRuns_PipelineSegment",
            table: "ScheduleRuns",
            column: "PipelineSegment");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "ScheduleRuns");
    }
}