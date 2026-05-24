using Microsoft.EntityFrameworkCore;
using TaskTrackerAPI.Data;
using TaskTrackerAPI.DTOs.Users;
using TaskTrackerAPI.Interfaces;

namespace TaskTrackerAPI.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<UserDto>> GetEmployeesAsync()
    {
        return await _db.Users
            .Where(u => u.Role == "Employee")
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                Role = u.Role
            })
            .ToListAsync();
    }
}