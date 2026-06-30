using BookingApi.Commands;
using BookingApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingApi.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginUserCommand command)
    {
        return Ok(await _userService.LoginAsync(command));
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserCommand command)
    {
        var user = await _userService.RegisterAsync(command);
        return Created($"/api/v1/auth/users/{user.Id}", user);
    }
}