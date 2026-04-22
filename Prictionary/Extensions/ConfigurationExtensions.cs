using Prictionary.Configuration;

namespace Prictionary.Extensions;

public static class ConfigurationExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddPrictionaryConfiguration(IConfiguration config)
        {
            services.Configure<AuthPolicy>(config.GetSection(AuthPolicy.Section));
            services.Configure<Secrets>(config.GetSection(Secrets.Section));
            services.Configure<StartupConfiguration>(config.GetSection(StartupConfiguration.Section));

            return services;
        }
    }
}
