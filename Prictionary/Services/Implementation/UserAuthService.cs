using Microsoft.AspNetCore.Identity;
using Prictionary.Configuration;
using Prictionary.DTOs;
using Prictionary.Models;
using Prictionary.Services.Infrastructure;
using Prictionary.Services.Interfaces;
using System.Security.Claims;

namespace Prictionary.Services.Implementation;

public class UserAuthService : IUserAuthService
{
    private const string
        InvalidCredentials = "Invalid credentials.",
        UnsupportedIdentificationType = "Provided identification type is not supported.",
        InvalidRefreshToken = "Provided refresh token is invalid.",
        UserWithProvidedRefreshTokenWasntFound = "User who was issued a refresh token doesn't exist.";

    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtService _jwtService;

    public UserAuthService(
        UserManager<AppUser> userManager,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    public async Task<Result<AuthenticationResponse, string>> AuthenticateAsync(CredentialsForm credentials)
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
                return new Result<AuthenticationResponse, string>(UnsupportedIdentificationType);
        }
        if (user is null)
        {
            return new Result<AuthenticationResponse, string>(InvalidCredentials);
        }
        var passwordCheckResult = await _userManager.CheckPasswordAsync(user, credentials.Password);
        if (!passwordCheckResult)
        {
            return new Result<AuthenticationResponse, string>(InvalidCredentials);
        }

        var (accessToken, refreshToken) = await _jwtService.GetAccessAndRefreshTokensAsync(user);
        var tokenResponse = new AuthenticationResponse 
        { 
            AccessToken = accessToken,
            RefreshToken = refreshToken 
        };
        return new Result<AuthenticationResponse, string>(tokenResponse);
    }

    public async Task<Result<AuthenticationResponse, string>> AuthenticateByRefreshTokenAsync(string refreshToken)
    {
        var tokenResult = _jwtService.ExtractClaimsFromToken(refreshToken);
        if (!tokenResult.Success)
            return new Result<AuthenticationResponse, string>(InvalidRefreshToken);
        var principal = tokenResult.Value!;

        string? userId = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimsIdentity.DefaultNameClaimType)?.Value;
        if (userId is null)
            return new Result<AuthenticationResponse, string>(InvalidRefreshToken);

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return new Result<AuthenticationResponse, string>(UserWithProvidedRefreshTokenWasntFound);

        (var accessToken, refreshToken) = await _jwtService.GetAccessAndRefreshTokensAsync(user);
        var tokenResponse = new AuthenticationResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
        return new Result<AuthenticationResponse, string>(tokenResponse);
    }
}
