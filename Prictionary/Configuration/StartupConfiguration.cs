namespace Prictionary.Configuration;

public class StartupConfiguration
{
    public AdminUserCredentials AdminUserCredentials { get; set; } = new AdminUserCredentials();
}

public class AdminUserCredentials
{
    public string Identificator { get; set; } = "admin";
    public string Password { get; set; } = "DefaultAdminPassword";
}
