using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GarageApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class addCommponSpecializations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"INSERT INTO Specialization (Id, Name) VALUES ('{Guid.NewGuid()}', 'Engine')");
            migrationBuilder.Sql($"INSERT INTO Specialization (Id, Name) VALUES ('{Guid.NewGuid()}', 'Transmission')");
            migrationBuilder.Sql($"INSERT INTO Specialization (Id, Name) VALUES ('{Guid.NewGuid()}', 'Chassis')");
            migrationBuilder.Sql($"INSERT INTO Specialization (Id, Name) VALUES ('{Guid.NewGuid()}', 'Brakes')");
            migrationBuilder.Sql($"INSERT INTO Specialization (Id, Name) VALUES ('{Guid.NewGuid()}', 'Exhaust')");
            migrationBuilder.Sql($"INSERT INTO Specialization (Id, Name) VALUES ('{Guid.NewGuid()}', 'Body')");
            migrationBuilder.Sql($"INSERT INTO Specialization (Id, Name) VALUES ('{Guid.NewGuid()}', 'Cleaning')");
            migrationBuilder.Sql($"INSERT INTO Specialization (Id, Name) VALUES ('{Guid.NewGuid()}', 'Tire fitting')");
            migrationBuilder.Sql($"INSERT INTO Specialization (Id, Name) VALUES ('{Guid.NewGuid()}', 'Electricity')");
            migrationBuilder.Sql($"INSERT INTO Specialization (Id, Name) VALUES ('{Guid.NewGuid()}', 'Steering')");
            migrationBuilder.Sql($"INSERT INTO Specialization (Id, Name) VALUES ('{Guid.NewGuid()}', 'Cooling System')");
            migrationBuilder.Sql($"INSERT INTO Specialization (Id, Name) VALUES ('{Guid.NewGuid()}', 'A\\C')");
            migrationBuilder.Sql($"INSERT INTO Specialization (Id, Name) VALUES ('{Guid.NewGuid()}', 'Fuel System')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
