using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NominationsApi.Migrations;

/// <inheritdoc />
public partial class AddScheduledVolume : Migration
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
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ScheduledVolume",
            table: "Nominations");
    }
}
