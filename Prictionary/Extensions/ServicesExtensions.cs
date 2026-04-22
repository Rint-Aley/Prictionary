using Prictionary.Models;
using Prictionary.Repositories.Implementions;
using Prictionary.Repositories.Interfaces;
using Prictionary.Services.Implementation;
using Prictionary.Services.Interfaces;
using Prictionary.ViewModels.Responses;

namespace Prictionary.Extensions;

public static class ServicesExtensions
{
    extension (IServiceCollection services)
    {
        public IServiceCollection AddPrictionaryServices()
        {
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUserAuthService, UserAuthService>();

            services.AddScoped<IAccessChecker<Group>, GroupAccessChecker>();
            services.AddScoped<IAccessChecker<LanguageUnit>, LanguageUnitsAccessChecker>();

            services.AddScoped<IGroupsRepository, IGroupsRepository>();
            services.AddScoped<ILanguageUnitsRepository, LanguageUnitsRepository>();
            services.AddScoped<IGroupLanguageUnitReferencesRepository, GroupLanguageUnitReferencesRepository>();

            return services;
        }
    }
}
