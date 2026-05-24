namespace TaskTrackerAPI.DTOs.Tasks
{
    public class TaskQueryParams
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortField { get; set; }
        public string? SortDir { get; set; } = "asc";
        public string? Search { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
    }
}
