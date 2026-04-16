namespace Prictionary.DTOs;

public class AuthenticationResponse
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}
