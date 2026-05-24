using System.ComponentModel.DataAnnotations;

namespace TaskTrackerAPI.DTOs.Tasks
{
    public class AddCommentDto
    {
        [Required, MinLength(1)]
        public string Text { get; set; } = string.Empty;
    }
}
