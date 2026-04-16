using Prictionary.DTOs;
using Prictionary.Services.Infrastructure;

namespace Prictionary.Services.Interfaces;

public interface IUserAuthService
{
    /// <summary>
    /// Attempts log in based on provided credentials; If successful, returns access and refresh tokens; otherwise error message.
    /// </summary>
    /// <param name="credentials">Credentials of user.</param>
    public Task<Result<AuthenticationResponse>> AuthenticateAsync(CredentialsForm credentials);

    /// <summary>
    /// Validates refresh token and issues a new pair of access and refresh tokens.
    /// </summary>
    /// <param name="refreshToken">Refresh token of user.</param>
    public Task<Result<AuthenticationResponse>> AuthenticateByRefreshTokenAsync(string refreshToken);
}
