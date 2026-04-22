using System.Text.Json.Serialization;

namespace Prictionary.Configuration;

public class AuthPolicy
{
    public const string Section = "AuthPolicy";

    public UserManagement UserManagement { get; set; } = UserManagement.Manual;
    public bool RestrictIdentificationType { get; set; } = true;
    public IdentificationType IdentificationType { get; set; } = IdentificationType.login;
    public JwtConfiguration JwtConfiguration { get; set; } = new JwtConfiguration();
}

public class JwtConfiguration
{
    public uint AccessTokenLifetimeInMinutes { get; set; } = 60;
    public uint RefreshTokenLifetimeInMinutes { get; set; } = 300;
}

public enum UserManagement
{
    /// <summary>
    /// Admin manually adds and deletes users
    /// </summary>
    Manual,
    /// <summary>
    /// Users can sing up by them self. Admins still can manage users.
    /// </summary>
    Decentalized
}

public enum IdentificationType
{
    login,
    email
}