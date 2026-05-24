using TaskTrackerAPI.DTOs.Auth;

namespace TaskTrackerAPI.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
        Task<AuthResponseDto> GoogleLoginAsync(GoogleLoginRequestDto dto);
    }
}
