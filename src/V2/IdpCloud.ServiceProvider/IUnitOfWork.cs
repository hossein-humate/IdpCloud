using IdpCloud.ServiceProvider.EntityService.BaseInfo;
using IdpCloud.ServiceProvider.EntityService.Identity;
using IdpCloud.ServiceProvider.EntityService.Security;
using IdpCloud.ServiceProvider.EntityService.SSO;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider
{
    public interface IUnitOfWork : IDisposable
    {
        IRoleRepository Roles { get; }
        IUserRepository Users { get; }
        ISoftwareRepository Softwares { get; }
        IUserSoftwareRepository UserSoftwares { get; }
        ILanguageRepository Languages { get; }
        IUserSessionRepository UserSessions { get; }
        IJwtSettingRepository JwtSettings { get; }
        IActivityRepository Activities { get; }
        IResetPasswordRepository ResetPasswords { get; }
        IBanListRepository BanLists { get; }

        int Complete();
        Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    }
}