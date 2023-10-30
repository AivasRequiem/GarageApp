using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GarageApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class addSpecializationToService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SpecializationId",
                table: "GarageService",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_GarageService_SpecializationId",
                table: "GarageService",
                column: "SpecializationId");

            migrationBuilder.AddForeignKey(
                name: "FK_GarageService_Specialization_SpecializationId",
                table: "GarageService",
                column: "SpecializationId",
                principalTable: "Specialization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarageService_Specialization_SpecializationId",
                table: "GarageService");

            migrationBuilder.DropIndex(
                name: "IX_GarageService_SpecializationId",
                table: "GarageService");

            migrationBuilder.DropColumn(
                name: "SpecializationId",
                table: "GarageService");
        }
    }
}