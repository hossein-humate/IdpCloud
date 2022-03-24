using IdpCloud.DataProvider.DatabaseContext;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.ServiceProvider.EntityService.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IdpCloud.Tests.Repositories
{
    /// <summary>
    /// Test suite for <see cref="NewUserSoftwareRepository"/>.
    /// </summary>
    public class UserSoftwareRepositoryTests
    {
        private readonly UserSoftwareRepository _userSoftwareRepository;
        private readonly Mock<EfCoreContext> _mockDbContext;
        private readonly Mock<DbSet<UserSoftware>> _mockUserSoftwareDbSet;
        private readonly DbContextHelper _dbContextHelper = new();
        private Guid userId1 = Guid.NewGuid();
        private Guid userId2 = Guid.NewGuid();
        private Guid userId3 = Guid.NewGuid();

        /// <summary>
        /// Initialises an instance of <see cref="UserSoftwareRepositoryTests"/>.
        /// </summary>
        public UserSoftwareRepositoryTests()
        {
            List<UserSoftware> _userSoftwares = GetUserSoftwares();
            var options = new DbContextOptionsBuilder<EfCoreContext>().UseSqlServer().Options;
            _mockDbContext = new Mock<EfCoreContext>(options);
            _mockUserSoftwareDbSet = _dbContextHelper.CreateDbSetMock(_userSoftwares.AsQueryable());
            _mockDbContext.Setup(x => x.UserSoftwares).Returns(_mockUserSoftwareDbSet.Object);
            _userSoftwareRepository = new UserSoftwareRepository(_mockDbContext.Object);
        }

        /// <summary>
        /// Asserts that
        /// <see cref = "NewUserSoftwareRepository.GetSoftwareListByUserId(Guid, System.Threading.CancellationToken)(Guid?, Status, CancellationToken)" />
        /// returns two software when User has access to two active and not deleted softwares
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task GetSoftwareListByUserId_ShouldReturnTwoSoftwares_ForUserId1ThatHasAccess()
        {
            //Act
            var softwares = await _userSoftwareRepository.GetSoftwareListByUserId(userId1);

            //Assert
            Assert.NotNull(softwares);
            Assert.Equal(2, softwares.Count());
        }

        /// <summary>
        /// Asserts that
        /// <see cref = "NewUserSoftwareRepository.GetSoftwareListByUserId(Guid, System.Threading.CancellationToken)(Guid?, Status, CancellationToken)" />
        /// returns zero software when User has access to deleted Software and UserSoftware records
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task GetSoftwareListByUserId_ShouldReturnEmpty_ForUserId2ThatHasDeletedAccessAndSoftware()
        {
            //Act
            var softwares = await _userSoftwareRepository.GetSoftwareListByUserId(userId2);

            //Assert
            Assert.NotNull(softwares);
            Assert.Empty(softwares);
        }

        /// <summary>
        /// Asserts that
        /// <see cref = "NewUserSoftwareRepository.GetSoftwareListByUserId(Guid, System.Threading.CancellationToken)(Guid?, Status, CancellationToken)" />
        /// returns empty when User has access to software but software status is Deactive right now.
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task GetSoftwareListByUserId_ShouldReturnEmpty_ForUserId3WithDeactiveSoftware()
        {
            //Act
            var softwares = await _userSoftwareRepository.GetSoftwareListByUserId(userId3);

            //Assert
            Assert.NotNull(softwares);
            Assert.Empty(softwares);
        }

        /// <summary>
        /// Asserts that
        /// <see cref = "NewUserSoftwareRepository.GetSoftwareListByUserId(Guid, System.Threading.CancellationToken)(Guid?, Status, CancellationToken)" />
        /// returns empty when User has not access to any software right now.
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task GetSoftwareListByUserId_ShouldReturnEmpty_ForUserId4NoAccessYet()
        {
            //Act
            var softwares = await _userSoftwareRepository.GetSoftwareListByUserId(userId3);

            //Assert
            Assert.NotNull(softwares);
            Assert.Empty(softwares);
        }

        private List<UserSoftware> GetUserSoftwares()
        {
            var software1 = new Software() { Name = "EVR" };
            var software2 = new Software() { Name = "Basemap IdP" };
            var software3 = new Software() { Name = "Data Cutter", DeleteDate = DateTime.UtcNow };
            var software4 = new Software() { Name = "TRACC", Status = SoftwareStatus.Deactive };

            return new List<UserSoftware>()
            {
                new UserSoftware()
                {
                    UserId = userId1,
                    Software = software1
                },
                new UserSoftware()
                {
                    UserId = userId1,
                    Software = software2
                },
                new UserSoftware()
                {
                    UserId = userId2,
                    Software = software1,
                    DeleteDate = DateTime.UtcNow
                },
                new UserSoftware()
                {
                    UserId = userId2,
                    Software = software3
                },
                new UserSoftware()
                {
                    UserId = userId3,
                    Software = software4
                }
            };
        }
    }
}
