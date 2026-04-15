using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prictionary.DTOs;

namespace Prictionary.ApiControllers;

[ApiController]
[ApiVersion("0")]
[Route("api/v{apiVersion:apiVersion}/[controller]")]
[Authorize]
public class GroupsController : ControllerBase
{
    [HttpPost]
    [MapToApiVersion("0")]
    public IActionResult PostGroup()
    {
        
        return Ok();
    }

    [HttpGet]
    [ApiVersion("0")]
    public List<GroupInfo> GetGroups()
    {
        return [];
    }

    [HttpGet("{groupId}")]
    [ApiVersion("0")]
    public GroupInfo GetGroup(int groupId)
    {
        return new GroupInfo() {
            Name = "",
            Description = ""
        };
    }
    
    [HttpPut("{groupId}")]
    [MapToApiVersion("0")]
    public IActionResult ChangeGroup(int groupId)
    {
        return Ok();
    }

    [HttpDelete("{groupId}")]
    [MapToApiVersion("0")]
    public IActionResult DeleteGroup(int groupId)
    {
        return Ok();
    }

    [HttpGet("{groupId}/words")]
    [MapToApiVersion("0")]
    public IActionResult GetWords(int groupId)
    {
        return Ok();
    }

    [HttpPost("{groupId}/words/{wordId}")]
    [MapToApiVersion("0")]
    public IActionResult GetWordsFromGroup(int groupId)
    {
        return Ok();
    }

    [HttpDelete("{groupId}/words/{wordId}")]
    [MapToApiVersion("0")]
    public IActionResult AddWordToGroup(int groupId, int wordId)
    {
        return Ok();
    }
}
