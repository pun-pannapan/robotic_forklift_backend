using Microsoft.AspNetCore.Mvc;
using Robotic.Forklift.Api.Controllers;
using Robotic.Forklift.Application.Dtos;
using Robotic.Forklift.Application.Interfaces;
using Robotic.Forklift.Backend.Tests;
using Robotic.Forklift.Domain.Entities;

namespace Robotic.Forklift.MSTests;

[TestClass]
public class AuthControllerTests
{
    private class FakeJwt : IJwtTokenService
    {
        public LoginResponse CreateToken(int userId, string username)
        { 
            return new LoginResponse("mockupjwttoken", DateTime.UtcNow.AddHours(1));
        }
    }

    [TestMethod]
    public async Task Login_success_returns_token()
    {
        using var db = TestSettings.NewDb(nameof(Login_success_returns_token));
        db.Users.Add(new User
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123")
        });
        await db.SaveChangesAsync();

        var ctrl = new AuthController(db, new FakeJwt());
        var result = await ctrl.Login(new LoginRequest("admin", "admin123"));

        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var ok = (OkObjectResult)result.Result!;
        var body = ok.Value as LoginResponse;
        Assert.IsNotNull(body);
        Assert.IsFalse(string.IsNullOrWhiteSpace(body!.Token));
    }

    [TestMethod]
    public async Task Login_invalid_returns_401()
    {
        using var db = TestSettings.NewDb(nameof(Login_invalid_returns_401));
        var ctrl = new AuthController(db, new FakeJwt());

        var result = await ctrl.Login(new LoginRequest("nope", "wrong"));
        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
    }
}
