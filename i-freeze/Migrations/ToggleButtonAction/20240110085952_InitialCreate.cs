using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace i_freeze.Migrations.ToggleButtonAction
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ToggleButtonAction",
                columns: table => new
                {
                    Toggle_Name = table.Column<string>(type: "TEXT", nullable: false),
                    Action = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToggleButtonAction", x => x.Toggle_Name);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ToggleButtonAction");
        }
    }
}
