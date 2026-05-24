using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using TaskTrackerAPI.Data;
using TaskTrackerAPI.DTOs.Auth;
using TaskTrackerAPI.Entities;
using TaskTrackerAPI.Exceptions;
using TaskTrackerAPI.Helpers;
using TaskTrackerAPI.Interfaces;

namespace TaskTrackerAPI.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly JwtHelper _jwt;

    public AuthService(AppDbContext db, JwtHelper jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

        return BuildResponse(user);
    }

    public async Task<AuthResponseDto> GoogleLoginAsync(GoogleLoginRequestDto dto)
    {
        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken);
        }
        catch
        {
            throw new UnauthorizedException("Invalid Google token.");
        }

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == payload.Email);

        if (user == null)
        {
            user = new User
            {
                Email = payload.Email,
                FullName = payload.Name ?? payload.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()),
                Role = "Employee",
                IsGoogleUser = true,
                GoogleId = payload.Subject
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        return BuildResponse(user);
    }

    private AuthResponseDto BuildResponse(User user) => new()
    {
        AccessToken = _jwt.GenerateToken(user),
        RefreshToken = _jwt.GenerateRefreshToken(),
        User = new UserInfoDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role
        }
    };
}