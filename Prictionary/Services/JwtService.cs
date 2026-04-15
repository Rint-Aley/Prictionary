using Microsoft.AspNetCore.Identity;
using Prictionary.Configuration;
using Prictionary.Models;
using Prictionary.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Prictionary.Services;

public class JwtService : IJwtService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AuthPolicy _authPolicy;

    public JwtService(
        UserManager<AppUser> userManager,
        AuthPolicy authPolicy)
    {
        _userManager = userManager;
        _authPolicy = authPolicy;
    }

    public async Task<(string, string)> GetAccessAndRefreshTokensAsync(AppUser user)
    {
        var accessToken = await GetAccessTokenAsync(user);
        var refreshToken = GetRefreshToken(user);

        return (accessToken, refreshToken);
    }

    public async Task<string> GetAccessTokenAsync(AppUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        List<Claim> claims = [
            new Claim(ClaimsIdentity.DefaultRoleClaimType, string.Join(",", roles))
        ];

        DateTime now = DateTime.UtcNow;

        var jwt = new JwtSecurityToken(
            notBefore: now,
            claims: claims,
            expires: now.AddMinutes(_authPolicy.JwtConfiguration.AccessTokenLifetimeInMinutes)
            //,
            //signingCredentials: GetSigningCredentials()
            );
        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        return token;
    }

    public string GetRefreshToken(AppUser user)
    {
        List<Claim> claims = [
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id),
            new Claim("purpose", "refresh")
        ];

        DateTime now = DateTime.UtcNow;

        var jwt = new JwtSecurityToken(
            notBefore: now,
            claims: claims,
            expires: now.AddMinutes(_authPolicy.JwtConfiguration.AccessTokenLifetimeInMinutes)
            //,
            //signingCredentials: GetSigningCredentials()
            );
        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        return token;
    }
}
