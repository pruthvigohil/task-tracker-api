namespace TaskTrackerAPI.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public TaskItem Task { get; set; } = null!;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
