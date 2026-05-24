using TaskTrackerAPI.DTOs.Users;

namespace TaskTrackerAPI.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetEmployeesAsync();
    }
}
