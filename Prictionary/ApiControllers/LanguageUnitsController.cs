using Asp.Versioning;
using FluentValidation.Resources;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Prictionary.Models;
using Prictionary.Repositories.Interfaces;
using Prictionary.Services.Interfaces;
using Prictionary.ViewModels.Responses;

namespace Prictionary.ApiControllers;

[ApiController]
[ApiVersion("0")]
[Route("api/v{apiVersion:apiVersion}/language-units")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class LanguageUnitsController : ControllerBase
{
    private readonly ILanguageUnitsRepository _languageUnitsRepository;
    private readonly IAccessChecker<LanguageUnit> _languageUnitsAccessChecker;
    private readonly UserManager<AppUser> _userManager;

    public LanguageUnitsController(
        ILanguageUnitsRepository languageUnitsRepository,
        IAccessChecker<LanguageUnit> languageUnitsAccessChecker,
        UserManager<AppUser> userManager)
    {
        _languageUnitsRepository = languageUnitsRepository;
        _languageUnitsAccessChecker = languageUnitsAccessChecker;
        _userManager = userManager;
    }

    [HttpGet("{id}")]
    [MapToApiVersion("0")]
    public async Task<IActionResult> GetWord(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        var languageUnit = await _languageUnitsRepository.GetByIdAsync(id);
        if (languageUnit is null)
            return NotFound();
        if (!_languageUnitsAccessChecker.CanRead(languageUnit, user.Id))
            return StatusCode(405);
        var response = new LanguageUnitResponse
        {
            Id = languageUnit.Id,
            Content = languageUnit.Content,
            AdditionalInformation = languageUnit.AdditionalInformation,
        };

        return Ok(response);
    }

    [HttpPut("{id}")]
    [MapToApiVersion("0")]
    public async Task<IActionResult> ChangeWord([FromBody] LanguageUnit newValues, int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        var languageUnit = await _languageUnitsRepository.GetByIdAsync(id);
        if (languageUnit is null)
            return NotFound();
        if (!_languageUnitsAccessChecker.CanChange(languageUnit, user.Id))
            return StatusCode(405);
        await _languageUnitsRepository.UpdateAsync(languageUnit.Id, newValues);

        return Ok(languageUnit);
    }
}
