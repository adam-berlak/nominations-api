using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NominationsApi.Migrations;

/// <inheritdoc />
public partial class _20261118000000_AddScheduledVolume : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "ScheduledVolume",
            table: "Nominations",
            type: "TEXT",
            precision: 18,
            scale: 4,
            nullable: false,
            defaultValue: 0m);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ScheduledVolume",
            table: "Nominations");
    }
}
