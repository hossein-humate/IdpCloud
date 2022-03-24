using AutoMapper;
using DataProvider.DatabaseContext;
using EntityServiceProvider.EntityService.BaseInfo;
using EntityServiceProvider.EntityService.Identity;
using EntityServiceProvider.EntityService.Log;
using System.Threading;
using System.Threading.Tasks;
using EntityServiceProvider.EntityService.SSO;

namespace EntityServiceProvider
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EfCoreContext _context;
        private readonly IMapper _mapper;

        public UnitOfWork(EfCoreContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region Identity Service Repositories
        private IAddressRepository _address;
        public IAddressRepository Addresses => _address ??= new AddressRepository(_context, _mapper);

        private IPersonRepository _person;
        public IPersonRepository Persons => _person ??= new PersonRepository(_context, _mapper);

        private IPermissionRepository _permission;
        public IPermissionRepository Permissions => _permission ??= new PermissionRepository(_context, _mapper);

        private IRolePermissionRepository _rolePermission;
        public IRolePermissionRepository RolePermissions => _rolePermission ??= new RolePermissionRepository(_context, _mapper);

        private IRoleRepository _role;
        public IRoleRepository Roles => _role ??= new RoleRepository(_context, _mapper);

        private ISocialNetworkRepository _socialNetwork;
        public ISocialNetworkRepository SocialNetworks => _socialNetwork ??= new SocialNetworkRepository(_context, _mapper);

        private IUserRepository _user;
        public IUserRepository Users => _user ??= new UserRepository(_context, _mapper);

        private IUserRoleRepository _userRole;
        public IUserRoleRepository UserRoles => _userRole ??= new UserRoleRepository(_context, _mapper);

        private IVisitorRepository _visitor;
        public IVisitorRepository Visitors => _visitor ??= new VisitorRepository(_context, _mapper);

        private ISoftwareRepository _software;
        public ISoftwareRepository Softwares => _software ??= new SoftwareRepository(_context, _mapper);

        private IUserSoftwareRepository _userSoftware;
        public IUserSoftwareRepository UserSoftwares => _userSoftware ??= new UserSoftwareRepository(_context, _mapper);
        #endregion Identity Service Repositories

        #region BaseInfo Service Repositories
        private ICountryRepository _countryRepository;
        public ICountryRepository Countries => _countryRepository ??= new CountryRepository(_context, _mapper);

        private ICurrencyRepository _currency;
        public ICurrencyRepository Currencies => _currency ??= new CurrencyRepository(_context, _mapper);

        private ICityRepository _city;
        public ICityRepository Cities => _city ??= new CityRepository(_context, _mapper);

        private ILanguageRepository _language;
        public ILanguageRepository Languages => _language ??= new LanguageRepository(_context, _mapper);

        private IMasterDetailRepository _masterDetail;
        public IMasterDetailRepository MasterDetails => _masterDetail ??= new MasterDetailRepository(_context, _mapper);
        #endregion BaseInfo Service Repositories

        #region Log Service Repositories
        private IServerActivityRepository _serverActivity;
        public IServerActivityRepository ServerActivities => _serverActivity ??= new ServerActivityRepository(_context, _mapper);
        #endregion Log Service Repositories

        #region SSO Service Repositories
        private IUserSessionRepository _userSession;
        public IUserSessionRepository UserSessions => _userSession ??= new UserSessionRepository(_context, _mapper);
        #endregion SSO Service Repositories

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
        }
    }
}