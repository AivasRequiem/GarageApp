using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GarageApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingSlots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "BookingSlot",
                columns: table => new
                {
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GarageServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingSlot", x => new { x.GarageServiceId, x.Date });
                    table.ForeignKey(
                        name: "FK_BookingSlot_GarageService_GarageServiceId",
                        column: x => x.GarageServiceId,
                        principalTable: "GarageService",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingSlot");
        }
    }
}
