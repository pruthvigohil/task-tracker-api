using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskTrackerAPI.DTOs.Tasks;
using TaskTrackerAPI.Interfaces;

namespace TaskTrackerAPI.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    // Admin: get all tasks
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll([FromQuery] TaskQueryParams queryParams)
    {
        var result = await _taskService.GetAllTasksAsync(queryParams);
        return Ok(result);
    }

    // Employee: get my tasks
    [HttpGet("my-tasks")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> GetMyTasks([FromQuery] TaskQueryParams queryParams)
    {
        var userId = GetUserId();
        var result = await _taskService.GetMyTasksAsync(userId, queryParams);
        return Ok(result);
    }

    // Both roles: get task by id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _taskService.GetTaskByIdAsync(id);
        return Ok(result);
    }

    // Admin only: create
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    {
        var result = await _taskService.CreateTaskAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // Admin only: update
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskDto dto)
    {
        var result = await _taskService.UpdateTaskAsync(id, dto);
        return Ok(result);
    }

    // Admin only: delete
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _taskService.DeleteTaskAsync(id);
        return NoContent();
    }

    // Employee only: update status
    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
    {
        var userId = GetUserId();
        var result = await _taskService.UpdateStatusAsync(id, userId, dto);
        return Ok(result);
    }

    // Both roles: add comment
    [HttpPost("{id}/comments")]
    public async Task<IActionResult> AddComment(int id, [FromBody] AddCommentDto dto)
    {
        var userId = GetUserId();
        var result = await _taskService.AddCommentAsync(id, userId, dto);
        return Ok(result);
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}