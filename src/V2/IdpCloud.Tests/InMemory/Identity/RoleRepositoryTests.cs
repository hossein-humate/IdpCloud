using IdpCloud.DataProvider.DatabaseContext;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.ServiceProvider.EntityService.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace IdpCloud.Tests.InMemory.Identity
{
    /// <summary>
    /// Test suite for <see cref="NewRoleRepository"/>.
    /// </summary>
    public class RoleRepositoryTests : TestBase
    {
        private readonly IRoleRepository _userRoleRepository;
        private readonly EfCoreContext _efDbContext;
        private List<UserRole> _userRoles;
        private Guid publicRole = Guid.NewGuid();

        /// <summary>
        /// Initialises an instance of <see cref="RoleRepositoryTests"/>.
        /// </summary>
        public RoleRepositoryTests()
        {
            _userRoles = GetUserRoles();
            _efDbContext = new EfCoreContext(GetMockContextOptions());
            _efDbContext.Database.EnsureDeleted();
            _efDbContext.Database.EnsureCreated();
            _efDbContext.UserRoles.AddRange(_userRoles);
            _efDbContext.SaveChanges();
            _userRoleRepository = new RoleRepository(_efDbContext);
        }

        /// <summary>
        /// Asserts that
        /// <see cref = "OrganisationRepository.Add(Organisation)" />
        /// returns Organisation after successfully added to Database with expected OrganisationId
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task AddUserRole_ShouldAddAndReturnUserRole_WhenGivenUserRole()
        {
            //Arrange
            var userRole = new UserRole
            {
                User = new User
                {
                    Username = "TestUsername"
                },
                RoleId = publicRole
            };

            //Act
            var resultUserRole = await _userRoleRepository.AddUserRole(userRole);

            //Assert

            Assert.NotNull(resultUserRole);
            Assert.NotNull(resultUserRole.User);
            Assert.Equal(userRole.User.Username, resultUserRole.User.Username);
            Assert.Equal(userRole.RoleId, resultUserRole.RoleId);
        }

        /// <summary>
        /// Asserts <see cref="NewRoleRepository.DeleteUserRole(UserRole)(UserRole)"/>
        /// should mark delete userRole
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task Delete_ShouldMarkUserRoleWithDeleteDate_WhenGivenUserRole()
        {
            //Arrange
            var userRole = new UserRole
            {
                User = new User
                {
                    Username = "TestUsername"
                },
                RoleId = publicRole
            };

            var userRoleAdd = await _userRoleRepository.AddUserRole(userRole);

            //Act
            _userRoleRepository.DeleteUserRole(userRoleAdd);

            //Assert
            Assert.NotNull(userRoleAdd.DeleteDate);
        }

        private List<UserRole> GetUserRoles()
        {
            var userRole1 = new UserRole() { User = new User { Username = "TestUsername1" }, RoleId = publicRole };
            var userRole2 = new UserRole() { User = new User { Username = "TestUsername2" }, RoleId = publicRole };
            var userRole3 = new UserRole() { User = new User { Username = "TestUsername3" }, RoleId = publicRole };
            return new List<UserRole>() { userRole1, userRole2, userRole3 };
        }
    }
}
