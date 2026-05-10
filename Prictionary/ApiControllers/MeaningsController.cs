using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Prictionary.DTOs;
using Prictionary.Models;
using Prictionary.Repositories.Interfaces;
using Prictionary.Services.Implementation;
using Prictionary.Services.Infrastructure;
using Prictionary.Services.Interfaces;
using Prictionary.ViewModels.Requests;
using Prictionary.ViewModels.Responses;

namespace Prictionary.ApiControllers;

[ApiController]
[ApiVersion("0")]
[Route("api/v{apiVersion:apiVersion}/meanings")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class MeaningsController : ControllerBase
{
    private readonly IMeaningsRepository _meaningsRepository;
    private readonly IMeaningsService _meaningsService;
    private readonly IAccessChecker<Meaning> _meaningsAccessChecker;

    private readonly UserManager<AppUser> _userManager;

    public MeaningsController(
        IMeaningsRepository meaningsRepository,
        IMeaningsService meaningsService,
        IAccessChecker<Meaning>  meaningsAccessChecker,
        UserManager<AppUser> userManager)
    {
        _meaningsRepository = meaningsRepository;
        _meaningsService = meaningsService;
        _meaningsAccessChecker = meaningsAccessChecker;
        _userManager = userManager;
    }

    [HttpGet("{meaningId}")]
    [MapToApiVersion("0")]
    public async Task<IActionResult> GetMeaningById(int meaningId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Unauthorized();

        var meaning = await _meaningsRepository.GetByIdAsync(meaningId);
        if (meaning is null)
            return NotFound();
        if (!await _meaningsAccessChecker.CanReadAsync(meaning, user.Id))
            return StatusCode(405);

        var meaningResponse = (MeaningResponse)meaning;

        return Ok(meaningResponse);
    }

    [HttpPut("{meaningId}")]
    [MapToApiVersion("0")]
    public async Task<IActionResult> ChangeMeaning([FromBody] MeaningRequest newValues, int meaningId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Unauthorized();

        var newValuesDto = (DTOs.MeaningValues)newValues;

        var result = await _meaningsService.UpdateMeaningByIdAsync(meaningId, newValuesDto, user.Id);
        if (result.IsError(out var error))
        {
            switch (error)
            {
                case ServiceErrors.NotFound:
                    return NotFound();
                case ServiceErrors.AccessDenied:
                    return StatusCode(405);
                default:
                    return BadRequest(error);
            }
        }
        var value = result.Value!;
        
        var response = new UpdateMeaningResponse
        {
            UpdatedMeaning = (MeaningResponse)value.Value,
            PrioritiesRebalanced = (value.Status == MeaningsUpdateResult.UpdateStatus.PrioritiesRebalanced)
        };

        return Ok(response);
    }

    [HttpDelete("{meaningId}")]
    [MapToApiVersion("0")]
    public async Task<IActionResult> DeleteMeaning(int meaningId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Unauthorized();

        var meaning = await _meaningsRepository.GetByIdAsync(meaningId);
        if (meaning is null)
            return NotFound();
        if (!await _meaningsAccessChecker.CanDeleteAsync(meaning, user.Id))
            return StatusCode(405);

        await _meaningsRepository.DeleteByIdAsync(meaningId);

        return NoContent();
    }
}
