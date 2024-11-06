using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Dal.Migrations
{
    /// <inheritdoc />
    public partial class EventUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FloorPlanUrl",
                schema: "vb8",
                table: "Properties",
                newName: "FeatureImageUrl");

            migrationBuilder.AddColumn<string>(
                name: "EventType",
                schema: "vb8",
                table: "PropertyEvents",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventType",
                schema: "vb8",
                table: "PropertyEvents");

            migrationBuilder.RenameColumn(
                name: "FeatureImageUrl",
                schema: "vb8",
                table: "Properties",
                newName: "FloorPlanUrl");
        }
    }
}
