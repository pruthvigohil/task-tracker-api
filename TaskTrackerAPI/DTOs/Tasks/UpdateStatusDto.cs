using System.ComponentModel.DataAnnotations;

namespace TaskTrackerAPI.DTOs.Tasks
{
    public class UpdateStatusDto
    {
        [Required, RegularExpression("Pending|In Progress|Completed|Overdue")]
        public string Status { get; set; } = string.Empty;
    }
}
