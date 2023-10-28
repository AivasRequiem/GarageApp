using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GarageApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            Guid garageUserRoleId = Guid.NewGuid();
            Guid garageOwnerRoleId = Guid.NewGuid();
            Guid adminRoleId = Guid.NewGuid();
            migrationBuilder.Sql($"INSERT INTO AspNetRoles (Id, Name, NormalizedName) VALUES ('{garageUserRoleId}', 'garageUser', 'GARAGEUSER')");
            migrationBuilder.Sql($"INSERT INTO AspNetRoles (Id, Name, NormalizedName) VALUES ('{garageOwnerRoleId}', 'garageOwner', 'GARAGEOWNER')");
            migrationBuilder.Sql($"INSERT INTO AspNetRoles (Id, Name, NormalizedName) VALUES ('{adminRoleId}', 'Admin', 'ADMIN')");

            Guid userId = Guid.NewGuid();
            migrationBuilder.Sql($"INSERT INTO AspNetUsers (Id, UserName, " +
                $"NormalizedUserName, Email, NormalizedEmail, PasswordHash, " +
                $"SecurityStamp, ConcurrencyStamp, EmailConfirmed, PhoneNumberConfirmed, TwoFactorEnabled, " +
                $"LockoutEnabled, AccessFailedCount) " +
                $"VALUES ('{userId}', 'test@email.com', 'TEST@EMAIL.COM', " +
                $"'test@email.com', 'TEST@EMAIL.COM'," +
                $"'AQAAAAIAAYagAAAAEG42Ug7hTfqxDvhdhes/2rEfq36A+23jq87Nf/PLPu24YbZ9ifWl4DnzmNdua/4R5Q=='," +
                $"'TFJ73JVFQ22CRHHJ5ABFWILBRH5KJY4Q', 'd69e1ac7-9ebd-435e-a206-7b2b68d42ce5', 'true', 'false', " +
                $"'false', 'true', '0')");

            migrationBuilder.Sql($"INSERT INTO AspNetUserRoles (UserId, RoleId) " +
                $"VALUES ('{userId}', '{adminRoleId}')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
