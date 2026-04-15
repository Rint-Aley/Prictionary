using Prictionary.DTOs;
using Prictionary.Services.Infrastructure;

namespace Prictionary.Services.Interfaces;

public interface IUsersService
{
    public Task<Result<AuthenticationResponse>> AuthenticateAsync(CredentialsForm credentials);
}
