using Microsoft.AspNetCore.Identity;
using Prictionary.Models;
using Prictionary.Services.Infrastructure;
using Prictionary.DTOs;
using Prictionary.Configuration;
using Prictionary.Services.Interfaces;

namespace Prictionary.Services;

public class UsersService : IUsersService
{
    const string InvalidCredentials = "Invalid credentials.",
        UnsupportedIdentificationType = "Provided identification type is not supported.";

    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtService _jwtService;

    public UsersService(
        UserManager<AppUser> userManager,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    public async Task<Result<AuthenticationResponse>> AuthenticateAsync(CredentialsForm credentials)
    {
        AppUser? user;
        switch (credentials.IdentificationType)
        {
            case IdentificationType.login:
                user = await _userManager.FindByNameAsync(credentials.Identifier);
                break;
            case IdentificationType.email:
                user = await _userManager.FindByEmailAsync(credentials.Identifier);
                break;
            default:
                return new Result<AuthenticationResponse>(UnsupportedIdentificationType);
        }
        if (user is null)
        {
            return new Result<AuthenticationResponse>(InvalidCredentials);
        }
        var passwordCheckResult = await _userManager.CheckPasswordAsync(user, credentials.Password);
        if (!passwordCheckResult)
        {
            return new Result<AuthenticationResponse>(InvalidCredentials);
        }

        var (accessToken, refreshToken) = await _jwtService.GetAccessAndRefreshTokensAsync(user);
        var tokenResponse = new AuthenticationResponse 
        { 
            AccessToken = accessToken,
            RefreshToken = refreshToken 
        };
        return new Result<AuthenticationResponse>(tokenResponse);
    }
}
