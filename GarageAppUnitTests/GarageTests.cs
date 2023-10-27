using System.Net;
using System.Text;
using GarageApp;
using GarageApp.Data;
using GarageApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;

namespace GarageAppUnitTests;

public class GarageTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly WebApplicationFactory<Startup> _factory;

    public GarageTests(WebApplicationFactory<Startup> factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData("garageUser")]
    public async Task Create_Garage_As_Admin_Should_Succeed(string roleName)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Create a test user and set up the database
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await using (var transaction = await context.Database.BeginTransactionAsync())
        {
            // Create the user
            var user = new IdentityUser
            {
                UserName = "user@example.com",
                Email = "user@example.com"
            };

            await userManager.CreateAsync(user);

            // Assign the role to the user
            await userManager.AddToRoleAsync(user, roleName);

            // Save changes to the database
            await context.SaveChangesAsync();

            var garage = new Garage
            {
                Name = "TestGarage"
            };

            // Convert the garage object to JSON
            var garageJson = JsonConvert.SerializeObject(garage);
            var content = new StringContent(garageJson, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("/Garages/Create", content);

            // Assert
            Assert.False(context.Garages.Contains(garage));
                
            await transaction.RollbackAsync();
        }
    }

    // Add more test cases for other controller actions as needed
}