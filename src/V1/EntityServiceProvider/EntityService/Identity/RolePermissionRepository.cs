using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataProvider.DatabaseContext;
using Entity.Identity;
using Microsoft.EntityFrameworkCore;

namespace EntityServiceProvider.EntityService.Identity
{
    public class RolePermissionRepository : Repository<RolePermission>, IRolePermissionRepository
    {
        private readonly IUserRoleRepository _userRoleRepository;
        public RolePermissionRepository(EfCoreContext databaseContext, IMapper mapper)
            : base(databaseContext, mapper)
        {
            _userRoleRepository = new UserRoleRepository(databaseContext, mapper);
        }

        public async Task<IEnumerable<Permission>> GetUnionPermissionsAsync(Guid userId, Guid softwareId,
            CancellationToken cancellationToken = default)
        {
            var permissions = new List<Permission>();
            foreach (var role in _userRoleRepository.QueryableContext.Where(ur =>
                ur.UserId == userId && ur.Role.SoftwareId == softwareId).Select(ur => ur.Role))
            {
                permissions = permissions.Union(await QueryableContext.Where(rp => rp.RoleId == role.RoleId)
                    .Select(p => p.Permission).ToListAsync(cancellationToken)).ToList();
            }

            return permissions;
        }
    }

    public interface IRolePermissionRepository : IRepository<RolePermission>
    {
        Task<IEnumerable<Permission>> GetUnionPermissionsAsync(Guid userId, Guid softwareId,
            CancellationToken cancellationToken = default);
    }
}