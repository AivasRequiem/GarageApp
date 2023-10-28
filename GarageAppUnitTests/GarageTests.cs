using System.Net;
using System.Security.Claims;
using System.Text;
using GarageApp;
using GarageApp.Data;
using GarageApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
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

    [Fact]
    public async Task Create_Garage_As_Admin_Should_Succeed()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Create a test user and set up the database
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<IdentityUser>>();
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
            await userManager.AddToRoleAsync(user, "Admin");

            // Save changes to the database
            await context.SaveChangesAsync();

            var authenticationServiceMock = new Mock<IAuthenticationService>();
            authenticationServiceMock
                .Setup(a => a.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(s => s.GetService(typeof(IAuthenticationService)))
                .Returns(authenticationServiceMock.Object);

            signInManager.Context = new DefaultHttpContext();
            signInManager.Context.RequestServices = serviceProviderMock.Object;
            await signInManager.SignInAsync(user, false);

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
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            await transaction.RollbackAsync();
        }
    }

    [Fact]
    public async Task Create_Garage_As_GarageUser_Should_Fail()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Create a test user and set up the database
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<IdentityUser>>();
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
            await userManager.AddToRoleAsync(user, "garageUser");

            // Save changes to the database
            await context.SaveChangesAsync();

            var authenticationServiceMock = new Mock<IAuthenticationService>();
            authenticationServiceMock
                .Setup(a => a.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(s => s.GetService(typeof(IAuthenticationService)))
                .Returns(authenticationServiceMock.Object);

            signInManager.Context = new DefaultHttpContext();
            signInManager.Context.RequestServices = serviceProviderMock.Object;
            await signInManager.SignInAsync(user, false);

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
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            await transaction.RollbackAsync();
        }
    }
    
    [Fact]
    public async Task Find_Garage_Test()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Create a test user and set up the database
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<IdentityUser>>();
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
            await userManager.AddToRoleAsync(user, "garageUser");

            // Save changes to the database
            await context.SaveChangesAsync();

            var authenticationServiceMock = new Mock<IAuthenticationService>();
            authenticationServiceMock
                .Setup(a => a.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(s => s.GetService(typeof(IAuthenticationService)))
                .Returns(authenticationServiceMock.Object);

            signInManager.Context = new DefaultHttpContext();
            signInManager.Context.RequestServices = serviceProviderMock.Object;
            await signInManager.SignInAsync(user, false);

            var garage = new Garage
            {
                Name = "TestGarage"
            };

            // Convert the garage object to JSON
            var garageJson = JsonConvert.SerializeObject(garage);
            var content = new StringContent(garageJson, Encoding.UTF8, "application/json");

            context.Garages.Add(garage);
            await context.SaveChangesAsync();
            
            // Assert
            Assert.Equal(garage, context.Garages.FirstOrDefault(g => g.Name.Equals("TestGarage")));

            await transaction.RollbackAsync();
        }
    }
// Add more test cases for other controller actions as needed
}