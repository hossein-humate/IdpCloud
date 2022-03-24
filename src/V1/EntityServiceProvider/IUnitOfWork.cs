using EntityServiceProvider.EntityService.BaseInfo;
using EntityServiceProvider.EntityService.Identity;
using EntityServiceProvider.EntityService.Log;
using System;
using System.Threading;
using System.Threading.Tasks;
using EntityServiceProvider.EntityService.SSO;

namespace EntityServiceProvider
{
    public interface IUnitOfWork : IDisposable
    {
        #region Identity Schema
        IAddressRepository Addresses { get; }
        IPersonRepository Persons { get; }
        IPermissionRepository Permissions { get; }
        IRoleRepository Roles { get; }
        IRolePermissionRepository RolePermissions { get; }
        ISocialNetworkRepository SocialNetworks { get; }
        IUserRepository Users { get; }
        IUserRoleRepository UserRoles { get; }
        IVisitorRepository Visitors { get; }
        ISoftwareRepository Softwares { get; }
        IUserSoftwareRepository UserSoftwares{ get; }
        #endregion

        #region BaseInfo Schema
        ICurrencyRepository Currencies { get; }
        ICountryRepository Countries { get; }
        ICityRepository Cities { get; }
        ILanguageRepository Languages { get; }
        IMasterDetailRepository MasterDetails { get; }
        #endregion

        #region Log Schema
        IServerActivityRepository ServerActivities { get; }
        #endregion

        #region SSO Schema
        IUserSessionRepository UserSessions { get; }
        #endregion

        int Complete();
        Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    }
}