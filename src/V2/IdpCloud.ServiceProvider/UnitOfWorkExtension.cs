using IdpCloud.ServiceProvider.EntityService.BaseInfo;
using IdpCloud.ServiceProvider.EntityService.Identity;
using IdpCloud.ServiceProvider.EntityService.Security;
using IdpCloud.ServiceProvider.EntityService.SSO;
using Microsoft.Extensions.DependencyInjection;

namespace IdpCloud.ServiceProvider
{
    public static class UnitOfWorkExtension
    {
        public static void AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISoftwareRepository, SoftwareRepository>();
            services.AddScoped<ISoftwareRepository, SoftwareRepository>();
            services.AddScoped<IUserSoftwareRepository, UserSoftwareRepository>();
            services.AddScoped<IUserSoftwareRepository, UserSoftwareRepository>();
            services.AddScoped<IOrganisationRepository, OrganisationRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<ILanguageRepository, LanguageRepository>();
            services.AddScoped<ILanguageRepository, LanguageRepository>();
            services.AddScoped<IUserSessionRepository, UserSessionRepository>();
            services.AddScoped<IUserSessionRepository, UserSessionRepository>();
            services.AddScoped<IJwtSettingRepository, JwtSettingRepository>();
            services.AddScoped<IJwtSettingRepository, JwtSettingRepository>();
            services.AddScoped<IResetPasswordRepository, ResetPasswordRepository>();
            services.AddScoped<IActivityRepository, ActivityRepository>();
            services.AddScoped<IBanListRepository, BanListRepository>();
        }
    }
}
