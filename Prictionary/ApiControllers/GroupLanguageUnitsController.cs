using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Prictionary.Models;
using Prictionary.Repositories.Interfaces;
using Prictionary.Services.Interfaces;
using Prictionary.ViewModels.Requests;

namespace Prictionary.ApiControllers;

[ApiController]
[ApiVersion("0")]
[Route("api/v{apiVersion:apiVersion}/groups/{groupId}/language-units")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class GroupLanguageUnitsController : ControllerBase
{
    private const string
        GroupWasNotFoundMessage = "Group with id {0} wasn't found.",
        LanguageUnitWasNotFoundMessage = "LanguageUnit with id {0} wasn't found.";

    private readonly IGroupsRepository _groupsRepository;
    private readonly ILanguageUnitsRepository _languageUnitsRepository;
    private readonly IGroupLanguageUnitReferencesRepository _groupLanguageUnitReferencesRepository;
    private readonly IAccessChecker<Group> _groupAccessChecker;
    private readonly IAccessChecker<LanguageUnit> _languageUnitAccessChecker;
    private readonly UserManager<AppUser> _userManager;

    public GroupLanguageUnitsController(
        IGroupsRepository groupsRepository,
        ILanguageUnitsRepository languageUnitsRepository,
        IGroupLanguageUnitReferencesRepository groupLanguageUnitReferencesRepository,
        IAccessChecker<Group> groupAccessChecker,
        IAccessChecker<LanguageUnit> languageUnitAccessChecker,
        UserManager<AppUser> userManager)
    {
        _groupsRepository = groupsRepository;
        _languageUnitsRepository = languageUnitsRepository;
        _groupLanguageUnitReferencesRepository = groupLanguageUnitReferencesRepository;
        _groupAccessChecker = groupAccessChecker;
        _languageUnitAccessChecker = languageUnitAccessChecker;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> PostNonexistentLanguageUnit([FromBody] CreateLanguageUnitRequest newLanguageUnit, int groupId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        var group = await _groupsRepository.GetByIdAsync(groupId);
        if (group is null)
            return NotFound(string.Format(GroupWasNotFoundMessage, groupId));
        if (!_groupAccessChecker.CanChange(group, user.Id))
            return StatusCode(405);

        var languageUnit = new LanguageUnit
        {
            Content = newLanguageUnit.Content,
            AdditionalInformation = newLanguageUnit.AdditionalInformation,
            Groups = [group]
        };
        await _languageUnitsRepository.CreateAsync(languageUnit);

        return Created();
    }

    [HttpPost("{languageUnitId}")]
    public async Task<IActionResult> PostExistingLanguageUnit(int groupId, int languageUnitId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        var group = await _groupsRepository.GetByIdAsync(groupId);
        if (group is null)
            return NotFound(string.Format(GroupWasNotFoundMessage, groupId));
        if (!_groupAccessChecker.CanChange(group, user.Id))
            return StatusCode(405);

        var languageUnit = await _languageUnitsRepository.GetByIdAsync(languageUnitId);
        if (languageUnit is null)
            return NotFound(string.Format(LanguageUnitWasNotFoundMessage, groupId));
        if (!_languageUnitAccessChecker.CanRead(languageUnit, user.Id))
            return StatusCode(405);

        await _groupLanguageUnitReferencesRepository.CreateAsync(group.Id, languageUnit.Id);

        return Created();
    }

    [HttpDelete("{languageUnitId}")]
    public async Task<IActionResult> DeleteLanguageUnitRelation(int groupId, int languageUnitId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        var group = await _groupsRepository.GetByIdAsync(groupId);
        if (group is null)
            return NotFound(string.Format(GroupWasNotFoundMessage, groupId));
        if (!_groupAccessChecker.CanChange(group, user.Id))
            return StatusCode(405);

        var languageUnit = await _languageUnitsRepository.GetByIdAsync(languageUnitId);
        if (languageUnit is null)
            return NotFound(string.Format(LanguageUnitWasNotFoundMessage, groupId));
        if (!_languageUnitAccessChecker.CanRead(languageUnit, user.Id))
            return StatusCode(405);

        await _groupLanguageUnitReferencesRepository.DeleteAsync(group.Id, languageUnit.Id);

        if (await _groupLanguageUnitReferencesRepository.GetRelationNumberAsync(languageUnit.Id) == 0)
        {
            await _languageUnitsRepository.DeleteAsync(languageUnit.Id);
        }

        return NoContent();
    }
}
