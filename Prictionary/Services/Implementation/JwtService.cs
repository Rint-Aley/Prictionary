using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Prictionary.Configuration;
using Prictionary.Models;
using Prictionary.Services.Infrastructure;
using Prictionary.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Prictionary.Services.Implementation;

public class JwtService : IJwtService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AuthPolicy _authPolicy;
    private readonly Secrets _secrets;

    public JwtService(
        UserManager<AppUser> userManager,
        IOptions<AuthPolicy> authPolicy,
        IOptions<Secrets> secrets)
    {
        _userManager = userManager;
        _authPolicy = authPolicy.Value;
        _secrets = secrets.Value;
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
            expires: now.AddMinutes(_authPolicy.JwtConfiguration.AccessTokenLifetimeInMinutes),
            signingCredentials: GetSigningCredentials());
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
            expires: now.AddMinutes(_authPolicy.JwtConfiguration.AccessTokenLifetimeInMinutes),
            signingCredentials: GetSigningCredentials());
        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        return token;
    }

    public Result<ClaimsPrincipal, string> ExtractClaimsFromToken(string token)
    {
        string configurationTokenKey = _secrets.JwtSecret;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configurationTokenKey));

        var validationParameters = new TokenValidationParameters
        {
            LifetimeValidator = (before, expires, token, param) => expires > DateTime.UtcNow,
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateActor = false,
            ValidateLifetime = true,
            IssuerSigningKey = securityKey,
            ValidateIssuerSigningKey = true
        };
        try
        {
            ClaimsPrincipal principal =
                new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out SecurityToken _);
            return new Result<ClaimsPrincipal, string>(principal);
        }
        catch (Exception ex)
        {
            return new Result<ClaimsPrincipal, string>(ex.Message);
        }
    }
    private SigningCredentials GetSigningCredentials()
    {
        string configurationTokenKey = _secrets.JwtSecret;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configurationTokenKey));
        return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    }
}
