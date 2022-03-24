using IdpCloud.DataProvider.DatabaseContext;
using IdpCloud.ServiceProvider.EntityService.BaseInfo;
using IdpCloud.ServiceProvider.EntityService.Identity;
using IdpCloud.ServiceProvider.EntityService.Security;
using IdpCloud.ServiceProvider.EntityService.SSO;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EfCoreContext _context;

        public UnitOfWork(EfCoreContext context,
            IRoleRepository roles,
            IUserRepository users,
            ISoftwareRepository softwares,
            IUserSoftwareRepository userSoftwares,
            ILanguageRepository languages,
            IUserSessionRepository userSessions,
            IJwtSettingRepository jwtSettings,
            IActivityRepository activities,
            IResetPasswordRepository resetPasswords,
            IBanListRepository banLists)
        {
            _context = context;
            Roles = roles;
            Users = users;
            Softwares = softwares;
            UserSoftwares = userSoftwares;
            Languages = languages;
            UserSessions = userSessions;
            JwtSettings = jwtSettings;
            Activities = activities;
            ResetPasswords = resetPasswords;
            BanLists = banLists;
        }

        public IRoleRepository Roles { get; }
        public IUserRepository Users { get; }
        public ISoftwareRepository Softwares { get; }
        public IUserSoftwareRepository UserSoftwares { get; }
        public ILanguageRepository Languages { get; }
        public IUserSessionRepository UserSessions { get; }
        public IJwtSettingRepository JwtSettings { get; }
        public IActivityRepository Activities { get; }
        public IResetPasswordRepository ResetPasswords { get; }
        public IBanListRepository BanLists { get; }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}