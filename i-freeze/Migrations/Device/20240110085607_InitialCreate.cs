using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace i_freeze.Migrations.Device
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceAction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeviceName = table.Column<string>(type: "TEXT", nullable: false),
                    Action = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceAction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeviceName = table.Column<string>(type: "TEXT", nullable: false),
                    IsAdminDevice = table.Column<string>(type: "TEXT", nullable: false),
                    OperatingSystemVersion = table.Column<string>(type: "TEXT", nullable: false),
                    DeviceIp = table.Column<string>(type: "TEXT", nullable: false),
                    MacAddress = table.Column<string>(type: "TEXT", nullable: false),
                    DisableUSB = table.Column<string>(type: "TEXT", nullable: false),
                    ActivateProactiveScan = table.Column<string>(type: "TEXT", nullable: false),
                    ActivateNetworkScan = table.Column<string>(type: "TEXT", nullable: false),
                    EnableUSBScan = table.Column<string>(type: "TEXT", nullable: false),
                    MuteMicrophone = table.Column<string>(type: "TEXT", nullable: false),
                    DisableCamera = table.Column<string>(type: "TEXT", nullable: false),
                    IsolateDevice = table.Column<string>(type: "TEXT", nullable: false),
                    BlockPowerShell = table.Column<string>(type: "TEXT", nullable: false),
                    DisableTethering = table.Column<string>(type: "TEXT", nullable: false),
                    BlockUntrustedIPs = table.Column<string>(type: "TEXT", nullable: false),
                    ActivateWhitelist = table.Column<string>(type: "TEXT", nullable: false),
                    TamperProtection = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceActions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceAction");

            migrationBuilder.DropTable(
                name: "DeviceActions");
        }
    }
}
