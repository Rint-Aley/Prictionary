using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Prictionary.Configuration;
using Prictionary.Models;
using Prictionary.Services;
using Prictionary.DTOs;
using Prictionary.Services.Interfaces;
using Prictionary.ViewModels;

namespace Prictionary.ApiControllers;

[ApiController]
[ApiVersion("0")]
[Route("api/v{apiVersion:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly AuthPolicy _authPolicy;
    private readonly IUsersService _usersService;

    public AuthController(
        SignInManager<AppUser> signInManager,
        IUsersService usersService,
        AuthPolicy authPolicy)
    {
        _signInManager = signInManager;
        _usersService = usersService;
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
        if (credentials.IdentificationType != _authPolicy.IdentificationType)
        {
            return BadRequest();
        }

        var result = await _usersService.AuthenticateAsync(credentials);
        if (!result.Success)
        {
            return BadRequest(result.Error);
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
        // validates refresh token
        // checks if it is the latest 
        // generates new access and refresh tokens
        throw new NotImplementedException();
    }

    [HttpPost("signup")]
    [MapToApiVersion("0")]
    public async Task<IActionResult> SignUp()
    {
        return Unauthorized();
    }
}
