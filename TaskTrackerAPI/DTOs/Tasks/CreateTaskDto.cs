using System.ComponentModel.DataAnnotations;

namespace TaskTrackerAPI.DTOs.Tasks
{
    public class CreateTaskDto
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required, RegularExpression("Low|Medium|High")]
        public string Priority { get; set; } = "Medium";

        [Required, RegularExpression("Pending|In Progress|Completed")]
        public string Status { get; set; } = "Pending";

        [Required]
        public DateTime DueDate { get; set; }

        public int? AssignedUserId { get; set; }
    }
}
