namespace TaskTrackerAPI.DTOs.Tasks
{
    public class PagedResultDto<T>
    {
        public List<T> Data { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
