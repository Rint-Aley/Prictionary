using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Prictionary.Models;
using Prictionary.Repositories;
using Prictionary.Repositories.Interfaces;
using Prictionary.Services.Interfaces;
using Prictionary.ViewModels.Requests;
using Prictionary.ViewModels.Responses;

namespace Prictionary.ApiControllers;

[ApiController]
[ApiVersion("0")]
[Route("api/v{apiVersion:apiVersion}/groups")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class GroupsController : ControllerBase
{
    private readonly IGroupsRepository _groupsRepository;
    private readonly IAccessChecker<Group> _groupAccessChecker;
    private readonly UserManager<AppUser> _userManager;
    private readonly LinkGenerator _linkGenerator;

    public GroupsController(
        IGroupsRepository groupsRepository,
        IAccessChecker<Group> groupAccessChecker,
        UserManager<AppUser> userManager,
        LinkGenerator linkGenerator)
    {
        _groupsRepository = groupsRepository;
        _groupAccessChecker = groupAccessChecker;
        _userManager = userManager;
        _linkGenerator = linkGenerator;
    }

    [HttpPost]
    [MapToApiVersion("0")]
    public async Task<IActionResult> PostGroup([FromBody] CreateGroupRequest createGroupRequest)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        var newGroup = new Group
        {
            Name = createGroupRequest.Name,
            Description = createGroupRequest.Description,
            User = user,
            UserId = user.Id,
        };
        var repositoryResult = await _groupsRepository.CreateGroupAsync(newGroup);
        if (repositoryResult.IsError(out RepositoryError error))
        {
            switch (error.Error) 
            {
                case QueryError.ConstraintViolated:
                    return BadRequest(error.Details);
                default:
                    throw new InvalidOperationException(error.Details);
            }
        }
        var group = repositoryResult.Value!;
        var response = new GroupResponse
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description
        };

        var uri = _linkGenerator.GetUriByAction(
            HttpContext,
            controller: nameof(GroupsController),
            action: nameof(GetGroup),
            values: new { groupId = group.Id }
        );

        return Created(uri, response);
    }

    [HttpGet]
    [ApiVersion("0")]
    public async Task<IActionResult> GetGroups()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        var groups = await _groupsRepository.GetGroupListForSpecificUserAsync(user.Id);
        var response = groups.Select(group => new GroupResponse
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description
        });

        return Ok(response);
    }

    [HttpGet("{groupId}")]
    [ApiVersion("0")]
    public async Task<IActionResult> GetGroup(int groupId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        var group = await _groupsRepository.GetByIdAsync(groupId);
        if (group is null)
            return NotFound();
        if (!_groupAccessChecker.CanRead(group, user.Id))
            return StatusCode(405);
        var response = new GroupResponse
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description
        };

        return Ok(response);
    }
    
    [HttpPut("{groupId}")]
    [MapToApiVersion("0")]
    public async Task<IActionResult> ChangeGroup([FromBody] Group newGroup, int groupId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        var group = await _groupsRepository.GetByIdAsync(groupId);
        if (group is null)
            return NotFound();
        if (!_groupAccessChecker.CanChange(group, user.Id))
            return StatusCode(405);
        var changeResult = await _groupsRepository.ChangeGroup(groupId, newGroup);
        if (changeResult.IsError(out var error))
            return BadRequest($"Failed to change resource: {error.Details}");

        return Ok(changeResult.Value);
    }

    [HttpDelete("{groupId}")]
    [MapToApiVersion("0")]
    public async Task<IActionResult> DeleteGroup(int groupId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        var group = await _groupsRepository.GetByIdAsync(groupId);
        if (group is null)
            return NoContent();
        if (!_groupAccessChecker.CanDelete(group, user.Id))
            return StatusCode(405);
        await _groupsRepository.DeleteGroupAsync(groupId);

        return NoContent();
    }

    [HttpGet("{groupId}/words")]
    [MapToApiVersion("0")]
    public async Task<IActionResult> GetWords(int groupId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        var group = await _groupsRepository.GetByIdAsync(groupId);
        if (group is null)
            return NotFound();
        if (!_groupAccessChecker.CanRead(group, user.Id))
            return StatusCode(405);
        var wordsIds = await _groupsRepository.GetWordsIdsAsync(groupId);

        return Ok(wordsIds);
    }
}
