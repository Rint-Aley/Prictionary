using Prictionary.Models;
using Prictionary.Services.Infrastructure;
using System.Security.Claims;

namespace Prictionary.Services.Interfaces;

public interface IJwtService
{
    /// <summary>
    /// Returns new access and refresh token for provided user.
    /// </summary>
    /// <param name="user">User to generate tokens for.</param>
    public Task<(string AccessToken, string RefreshToken)> GetAccessAndRefreshTokensAsync(AppUser user);

    /// <summary>
    /// Returns new access token for provided user.
    /// </summary>
    /// <param name="user">User to generate tokens for.</param>
    public Task<string> GetAccessTokenAsync(AppUser user);

    /// <summary>
    /// Returns new refresh token for provided user.
    /// </summary>
    /// <param name="user">User to generate tokens for.</param>
    public string GetRefreshToken(AppUser user);

    /// <summary>
    /// Decrypts provided token and returns ClaimPrincipal encoded in it.
    /// </summary>
    /// <param name="token">JWT to be decrypted.</param>
    public Result<ClaimsPrincipal, string> ExtractClaimsFromToken(string token);
}
