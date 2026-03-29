using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Prictionary.ApiControllers;

[ApiController]
[ApiVersion("0")]
[Route("api/v{apiVersion:apiVersion}/[controller]")]
public class WordsController : ControllerBase
{
    [HttpPost]
    [MapToApiVersion("0")]
    public IActionResult PostWord()
    {
        return Created();
    }

    [HttpGet("{id}")]
    [MapToApiVersion("0")]
    public IActionResult GetWord(int id)
    {
        return Ok(new { words = new[] { "hello", "world" } });
    }

    [HttpPut("{id}")]
    [MapToApiVersion("0")]
    public IActionResult ChangeWord(int id)
    {
        return Ok();
    }

    [HttpDelete("{id}")]
    [MapToApiVersion("0")]
    public IActionResult DeleteWord(int id)
    {
        return Ok();
    }
}
