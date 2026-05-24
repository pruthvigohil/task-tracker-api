using Microsoft.AspNetCore.Mvc;
using TaskTrackerAPI.Data;
using TaskTrackerAPI.DTOs.Auth;
using TaskTrackerAPI.Interfaces;

namespace TaskTrackerAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return Ok(result);
    }

    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequestDto dto)
    {
        var result = await _authService.GoogleLoginAsync(dto);
        return Ok(result);
    }

    // TEMPORARY — delete after seeding
    [HttpGet("seed")]
    public async Task<IActionResult> Seed([FromServices] AppDbContext db)
    {
        if (!db.Users.Any())
        {
            db.Users.AddRange(
                new TaskTrackerAPI.Entities.User
                {
                    Email = "admin@test.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    FullName = "Admin User",
                    Role = "Admin"
                },
                new TaskTrackerAPI.Entities.User
                {
                    Email = "employee@test.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("emp123"),
                    FullName = "Employee User",
                    Role = "Employee"
                }
            );
            await db.SaveChangesAsync();
        }
        return Ok("Seeded.");
    }
}