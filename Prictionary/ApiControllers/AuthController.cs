using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Prictionary.Configuration;
using Prictionary.DTOs;
using Prictionary.Models;
using Prictionary.Services;
using Prictionary.Services.Infrastructure;
using Prictionary.Services.Interfaces;
using Prictionary.ViewModels;
using System.Net;

namespace Prictionary.ApiControllers;

[ApiController]
[ApiVersion("0")]
[Route("api/v{apiVersion:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    private const string MismatchInIdentificationTypeMessage = "Provided authentication type is not supported by cofiguration of application.";

    private readonly SignInManager<AppUser> _signInManager;
    private readonly AuthPolicy _authPolicy;
    private readonly IUserAuthService _userAuthService;

    public AuthController(
        SignInManager<AppUser> signInManager,
        IUserAuthService userAuthService,
        AuthPolicy authPolicy)
    {
        _signInManager = signInManager;
        _userAuthService = userAuthService;
        _authPolicy = authPolicy;
    }

    [HttpPost("login")]
    [MapToApiVersion("0")]
    public async Task<IActionResult> LogIn([FromBody] CredentialsForm credentials, CredentialsFormValidator validator)
    {
        if (!validator.Validate(credentials).IsValid)
        {
            return BadRequest(ModelState);
        }
        if (_authPolicy.RestrictIdentificationType && _authPolicy.IdentificationType != credentials.IdentificationType)
        {
            return BadRequest(MismatchInIdentificationTypeMessage);
        }

        var result = await _userAuthService.AuthenticateAsync(credentials);
        if (!result.Success)
        {
            return Unauthorized(result.Error);
        }

        var loginResponse = new LoginResponse
        {
            AccessToken = result.Value!.AccessToken,
            RefreshToken = result.Value!.RefreshToken,
        };

        return Ok(loginResponse);
    }

    [HttpPost("refresh")]
    [MapToApiVersion("0")]
    public async Task<IActionResult> RefreshToken()
    {
        if (!Request.Cookies.TryGetValue(Constants.TokenConstants.REFRESH_TOKEN_COOKIE_NAME, out string? refreshToken))
            return BadRequest("Refresh token is missing");

        var result = await _userAuthService.AuthenticateByRefreshTokenAsync(refreshToken);
        if (!result.Success)
        {
            return Unauthorized(result.Error);
        }

        var loginResponse = new LoginResponse
        {
            AccessToken = result.Value!.AccessToken,
            RefreshToken = result.Value!.RefreshToken,
        };
        
        return Ok(loginResponse);
    }
}
