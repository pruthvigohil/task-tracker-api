using TaskTrackerAPI.DTOs.Tasks;

namespace TaskTrackerAPI.Interfaces
{
    public interface ITaskService
    {
        Task<PagedResultDto<TaskDto>> GetAllTasksAsync(TaskQueryParams queryParams);
        Task<PagedResultDto<TaskDto>> GetMyTasksAsync(int userId, TaskQueryParams queryParams);
        Task<TaskDto> GetTaskByIdAsync(int id);
        Task<TaskDto> CreateTaskAsync(CreateTaskDto dto);
        Task<TaskDto> UpdateTaskAsync(int id, UpdateTaskDto dto);
        Task DeleteTaskAsync(int id);
        Task<TaskDto> UpdateStatusAsync(int id, int userId, UpdateStatusDto dto);
        Task<CommentDto> AddCommentAsync(int taskId, int userId, AddCommentDto dto);
    }
}
