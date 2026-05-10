using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Prictionary.DTOs;
using Prictionary.Models;
using Prictionary.Repositories.Interfaces;
using Prictionary.Services.Infrastructure;
using Prictionary.Services.Interfaces;
using Prictionary.ViewModels.Requests;
using Prictionary.ViewModels.Responses;

namespace Prictionary.ApiControllers;

[ApiController]
[ApiVersion("0")]
[Route("api/v{apiVersion:apiVersion}/language-units/{languageUnitId}/meanings")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class LanguageUnitMeaningsController : ControllerBase
{
    private readonly IMeaningsRepository _meaningsRepository;
    private readonly IMeaningsService _meaningsService;
    private readonly ILanguageUnitsRepository _languageUnitsRepository;
    private readonly IAccessChecker<LanguageUnit> _languageUnitsAccessChecker;

    private readonly UserManager<AppUser> _userManager;
    private readonly LinkGenerator _linkGenerator;

    public LanguageUnitMeaningsController(
        IMeaningsRepository meaningsRepository,
        IMeaningsService meaningsService,
        ILanguageUnitsRepository languageUnitsRepository,
        IAccessChecker<LanguageUnit> languageUnitsAccessChecker,
        UserManager<AppUser> userManager,
        LinkGenerator linkGenerator)
    {
        _meaningsRepository = meaningsRepository;
        _meaningsService = meaningsService;
        _languageUnitsRepository = languageUnitsRepository;
        _languageUnitsAccessChecker = languageUnitsAccessChecker;
        _userManager = userManager;
        _linkGenerator = linkGenerator;
    }

    [HttpPost]
    [MapToApiVersion("0")]
    public async Task<IActionResult> CreateMeaning([FromBody] MeaningRequest request, int languageUnitId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Unauthorized();

        var languageUnit = await _languageUnitsRepository.GetByIdAsync(languageUnitId);
        if (languageUnit is null)
            return NotFound();
        if (!await _languageUnitsAccessChecker.CanReadAsync(languageUnit, user.Id))
            return StatusCode(405);

        var position = request.Position is null ? null : (DTOs.Position)request.Position;
        var result = await _meaningsService.CreateMeaningAsync(languageUnitId, request.Content, position);
        if (result.IsError(out var error))
        {
            return error switch
            {
                ServiceErrors.NotFound => NotFound(),
                _ => BadRequest(error),
            };
        }

        var created = result.Value!;
        var uri = _linkGenerator.GetUriByAction(
            HttpContext,
            controller: nameof(MeaningsController),
            action: nameof(MeaningsController.GetMeaningById),
            values: new { meaningId = created.Id }
        );

        return Created(uri, (MeaningResponse)created);
    }

    [HttpGet]
    [MapToApiVersion("0")]
    public async Task<IActionResult> GetMeaningsForLanguageUnit(int languageUnitId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Unauthorized();

        var languageUnit = await _languageUnitsRepository.GetByIdAsync(languageUnitId);
        if (languageUnit is null)
            return NotFound();
        if (!await _languageUnitsAccessChecker.CanReadAsync(languageUnit, user.Id))
            return StatusCode(405);

        var meanings = await _meaningsRepository.GetMeaningsOfLanguageUnitAsync(languageUnit.Id);

        var meaningsViewModel = meanings.Select(meaning => (MeaningResponse)meaning);

        return Ok(meaningsViewModel);
    }
}
