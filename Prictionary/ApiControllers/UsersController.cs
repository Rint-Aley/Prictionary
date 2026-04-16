using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prictionary.Models;

namespace Prictionary.ApiControllers;

[ApiController]
[ApiVersion("0")]
[Route("api/v{apiVersion:apiVersion}/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet]
    [MapToApiVersion("0")]
    [Authorize(Roles = "Admin")]
    public async Task<List<AppUser>> GetUsersListAsync()
    {
        return [];
    }

    [HttpPost]
    [MapToApiVersion("0")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddUserAsync()
    {
        return Ok();
    }

    [HttpDelete("{userId}")]
    [MapToApiVersion("0")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUserAsync(string userId)
    {
        return Ok();
    }

    [HttpPut("{userId}")]
    [MapToApiVersion("0")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ChangeUserAsync(string userId)
    {
        return Ok();
    }
}
