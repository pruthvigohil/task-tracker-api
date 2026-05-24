using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTrackerAPI.Interfaces;

namespace TaskTrackerAPI.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("employees")]
    public async Task<IActionResult> GetEmployees()
    {
        var result = await _userService.GetEmployeesAsync();
        return Ok(result);
    }
}