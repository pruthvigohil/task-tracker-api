using Microsoft.EntityFrameworkCore;
using TaskTrackerAPI.Data;
using TaskTrackerAPI.DTOs.Tasks;
using TaskTrackerAPI.Entities;
using TaskTrackerAPI.Exceptions;
using TaskTrackerAPI.Interfaces;

namespace TaskTrackerAPI.Services;

public class TaskService : ITaskService
{
    private readonly AppDbContext _db;

    public TaskService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResultDto<TaskDto>> GetAllTasksAsync(TaskQueryParams q)
    {
        var query = _db.Tasks
            .Include(t => t.AssignedUser)
            .Include(t => t.Comments).ThenInclude(c => c.User)
            .AsQueryable();

        query = ApplyOverdueCheck(query);
        query = ApplySearch(query, q.Search);
        query = ApplyFilters(query, q);
        query = ApplySort(query, q.SortField, q.SortDir);

        var total = await query.CountAsync();
        var data = await query
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .Select(t => MapToDto(t))
            .ToListAsync();

        return new PagedResultDto<TaskDto>
        {
            Data = data,
            Total = total,
            Page = q.Page,
            PageSize = q.PageSize
        };
    }

    public async Task<PagedResultDto<TaskDto>> GetMyTasksAsync(int userId, TaskQueryParams q)
    {
        var query = _db.Tasks
            .Include(t => t.AssignedUser)
            .Include(t => t.Comments).ThenInclude(c => c.User)
            .Where(t => t.AssignedUserId == userId)
            .AsQueryable();

        query = ApplyOverdueCheck(query);
        query = ApplySearch(query, q.Search);
        query = ApplySort(query, q.SortField, q.SortDir);

        var total = await query.CountAsync();
        var data = await query
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .Select(t => MapToDto(t))
            .ToListAsync();

        return new PagedResultDto<TaskDto>
        {
            Data = data,
            Total = total,
            Page = q.Page,
            PageSize = q.PageSize
        };
    }

    public async Task<TaskDto> GetTaskByIdAsync(int id)
    {
        var task = await _db.Tasks
            .Include(t => t.AssignedUser)
            .Include(t => t.Comments).ThenInclude(c => c.User)
            .FirstOrDefaultAsync(t => t.Id == id)
            ?? throw new NotFoundException($"Task with id {id} not found.");

        return MapToDto(task);
    }

    public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto)
    {
        var duplicate = await _db.Tasks
            .AnyAsync(t => t.Title.ToLower() == dto.Title.ToLower());

        if (duplicate)
            throw new ConflictException("A task with this title already exists.");

        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            Status = dto.Status,
            DueDate = dto.DueDate,
            AssignedUserId = dto.AssignedUserId
        };

        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();
        return await GetTaskByIdAsync(task.Id);
    }

    public async Task<TaskDto> UpdateTaskAsync(int id, UpdateTaskDto dto)
    {
        var task = await _db.Tasks.FindAsync(id)
            ?? throw new NotFoundException($"Task with id {id} not found.");

        var duplicate = await _db.Tasks
            .AnyAsync(t => t.Title.ToLower() == dto.Title.ToLower() && t.Id != id);

        if (duplicate)
            throw new ConflictException("A task with this title already exists.");

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Priority = dto.Priority;
        task.Status = dto.Status;
        task.DueDate = dto.DueDate;
        task.AssignedUserId = dto.AssignedUserId;
        task.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return await GetTaskByIdAsync(task.Id);
    }

    public async Task DeleteTaskAsync(int id)
    {
        var task = await _db.Tasks.FindAsync(id)
            ?? throw new NotFoundException($"Task with id {id} not found.");

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
    }

    public async Task<TaskDto> UpdateStatusAsync(int id, int userId, UpdateStatusDto dto)
    {
        var task = await _db.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.AssignedUserId == userId)
            ?? throw new NotFoundException("Task not found or not assigned to you.");

        task.Status = dto.Status;
        task.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return await GetTaskByIdAsync(task.Id);
    }

    public async Task<CommentDto> AddCommentAsync(int taskId, int userId, AddCommentDto dto)
    {
        var taskExists = await _db.Tasks.AnyAsync(t => t.Id == taskId);
        if (!taskExists) throw new NotFoundException("Task not found.");

        var comment = new Comment
        {
            TaskId = taskId,
            UserId = userId,
            Text = dto.Text
        };

        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();

        await _db.Entry(comment).Reference(c => c.User).LoadAsync();

        return new CommentDto
        {
            Id = comment.Id,
            UserId = comment.UserId,
            UserName = comment.User.FullName,
            Text = comment.Text,
            CreatedAt = comment.CreatedAt
        };
    }

    // ── Private helpers ──────────────────────────────────────

    private IQueryable<TaskItem> ApplyOverdueCheck(IQueryable<TaskItem> query)
    {
        var now = DateTime.UtcNow;
        return query.Where(t =>
            t.Status != "Overdue" ||
            t.DueDate >= now ||
            t.Status == "Completed"
        );
        // We handle overdue in the DTO mapping below
    }

    private IQueryable<TaskItem> ApplySearch(IQueryable<TaskItem> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search)) return query;
        var s = search.ToLower();
        return query.Where(t =>
            t.Title.ToLower().Contains(s) ||
            t.Description.ToLower().Contains(s) ||
            (t.AssignedUser != null && t.AssignedUser.FullName.ToLower().Contains(s)));
    }

    private IQueryable<TaskItem> ApplyFilters(IQueryable<TaskItem> query, TaskQueryParams q)
    {
        if (!string.IsNullOrWhiteSpace(q.Status))
            query = query.Where(t => t.Status == q.Status);
        if (!string.IsNullOrWhiteSpace(q.Priority))
            query = query.Where(t => t.Priority == q.Priority);
        return query;
    }

    private IQueryable<TaskItem> ApplySort(IQueryable<TaskItem> query, string? field, string? dir)
    {
        bool desc = dir?.ToLower() == "desc";
        return field?.ToLower() switch
        {
            "title" => desc ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
            "priority" => desc ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority),
            "status" => desc ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status),
            "duedate" => desc ? query.OrderByDescending(t => t.DueDate) : query.OrderBy(t => t.DueDate),
            _ => query.OrderByDescending(t => t.CreatedAt)
        };
    }

    private static TaskDto MapToDto(TaskItem t)
    {
        var effectiveStatus = (t.Status != "Completed" && t.DueDate < DateTime.UtcNow)
            ? "Overdue" : t.Status;

        return new TaskDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Priority = t.Priority,
            Status = effectiveStatus,
            DueDate = t.DueDate,
            AssignedUserId = t.AssignedUserId,
            AssignedUserName = t.AssignedUser?.FullName ?? string.Empty,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt,
            Comments = t.Comments.Select(c => new CommentDto
            {
                Id = c.Id,
                UserId = c.UserId,
                UserName = c.User?.FullName ?? string.Empty,
                Text = c.Text,
                CreatedAt = c.CreatedAt
            }).ToList()
        };
    }
}