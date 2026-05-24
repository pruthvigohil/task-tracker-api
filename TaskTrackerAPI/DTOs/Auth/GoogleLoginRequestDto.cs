using System.ComponentModel.DataAnnotations;

namespace TaskTrackerAPI.DTOs.Auth
{
    public class GoogleLoginRequestDto
    {
        [Required]
        public string IdToken { get; set; } = string.Empty;
    }
}
