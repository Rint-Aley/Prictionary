using Prictionary.Models;
using Prictionary.Services.Infrastructure;

namespace Prictionary.Services.Interfaces;

public interface IJwtService
{
    public Task<(string AccessToken, string RefreshToken)> GetAccessAndRefreshTokensAsync(AppUser user);
    public Task<string> GetAccessTokenAsync(AppUser user);
    public string GetRefreshToken(AppUser user);
}
