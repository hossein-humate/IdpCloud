using IdpCloud.DataProvider.DatabaseContext;
using IdpCloud.DataProvider.Entity.BaseInfo;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.ServiceProvider.EntityService.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace IdpCloud.Tests.InMemory.Identity
{
    /// <summary>
    /// Test suite for <see cref="NewUserRepository"/>.
    /// </summary>
    public class UserRepositoryTests : TestBase
    {
        private readonly IUserRepository _newUserRepository;
        private readonly EfCoreContext _efDbContext;

        private readonly Guid _userId = Guid.NewGuid();
        private readonly int _OrganisationId = 1;
        private readonly Guid _softwareId = Guid.NewGuid();
        private readonly string _userName = "TestUserName";
        private readonly string _userEmail = "TestEmail";
        private readonly string _password = "TestPassword";
        private readonly string _userEmailSecret = "TestEmailSecret";
        /// <summary>
        /// Initialises an instance of <see cref="UserRepositoryTests"/>.
        /// </summary>
        public UserRepositoryTests()
        {
            _efDbContext = new EfCoreContext(GetMockContextOptions());
            _efDbContext.Database.EnsureDeleted();
            _efDbContext.Database.EnsureCreated();
            _newUserRepository = new UserRepository(_efDbContext);
        }

        /// <summary>
        /// Asserts <see cref="NewUserSessionRepository.GetUserSessionByIdAndStatus(Guid?, Status, CancellationToken)"/>
        /// returns UserSession</summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task GetAll_ShouldReturnAllUsers()
        {
            //Arrange
            AddUser();

            //Act
            var userList = await _newUserRepository.GetAll();

            //Arrange
            Assert.Single(userList);
            var user = userList.First();
            Assert.NotNull(user);

            var userSoftwares = user.UserSoftwares;
            Assert.Single(userSoftwares);

            var userSoftware = user.UserSoftwares.First();
            Assert.Null(userSoftware.DeleteDate);
            Assert.Equal(userSoftware.SoftwareId, _softwareId);

            Assert.Equal(user.UserId, _userId);
            Assert.Equal(123, (short)user.LanguageId);
        }

        /// <summary>
        /// Asserts <see cref="NewUserRepository.GetUserByUserNameOrEmailAndPassword(string, string, System.Threading.CancellationToken)"/>
        /// <see cref="usernameOrEmail">UserName</see>
        /// <see cref="password">user valid password</see>
        /// <see cref="cancellationToken"/>
        /// returns valid user
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task GetUserByUserNameOrEmailAndPassword_ShouldReturnValidUser_WhenGivenValidUserName()
        {
            //Arrange
            AddUser();

            //Act
            var user = await _newUserRepository.GetUserByUserNameOrEmailAndPassword(_userName, _password, default);

            //Assert
            Assert.NotNull(user);
            Assert.NotNull(user.Organisation);
            Assert.Equal(user.Username, _userName);
            Assert.Equal(user.Organisation.OrganisationId, _OrganisationId);
            Assert.Equal("Public", user.UserRoles.FirstOrDefault()?.Role?.Name);
            Assert.Equal(user.UserId, _userId);
        }

        /// <summary>
        /// Asserts <see cref="NewUserRepository.GetUserByUserNameOrEmailAndPassword(string, string, System.Threading.CancellationToken)"/>
        /// <see cref="usernameOrEmail">Email</see>
        /// <see cref="password">user valid password</see>
        /// <see cref="cancellationToken"/>
        /// returns valid user
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task GetUserByUserNameOrEmailAndPassword_ShouldReturnValidUser_WhenGivenValidUserEmail()
        {
            //Arrange
            AddUser();

            //Act
            var user = await _newUserRepository.GetUserByUserNameOrEmailAndPassword(_userEmail, _password, default);

            //Assert
            Assert.NotNull(user);
            Assert.NotNull(user.Organisation);
            Assert.Equal(user.Email, _userEmail);
            Assert.Equal(user.Organisation.OrganisationId, _OrganisationId);
            Assert.Equal("Public", user.UserRoles.FirstOrDefault()?.Role?.Name);
            Assert.Equal(user.UserId, _userId);
        }

        /// <summary>
        /// Asserts <see cref="NewUserRepository.GetByEmail(string, System.Threading.CancellationToken)"/>
        /// <see cref="User.Email">Email</see>
        /// should return a User when given valid Email which exits
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task GetByEmail_ShouldReturnUser_WhenGiveValidEmail()
        {
            //Arrange
            AddUser();
            var cancellationToken = CancellationToken.None;

            //Act
            var user = await _newUserRepository.GetByEmail(_userEmail, cancellationToken);

            //Assert
            Assert.NotNull(user);
            Assert.Equal(user.Email, _userEmail);
        }

        /// <summary>
        /// Asserts <see cref="NewUserRepository.GetByUsername(string, CancellationToken)"/>
        /// <see cref="User.Username"> Username of the user</see>
        /// should return a User when given valid username which exits
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task GetByUsername_ShouldReturnUser_WhenGiveValidUsernmae()
        {
            //Arrange
            AddUser();
            var cancellationToken = CancellationToken.None;

            //Act
            var user = await _newUserRepository.GetByUsername(_userName, cancellationToken);

            //Assert
            Assert.NotNull(user);
            Assert.Equal(user.Username, _userName);
        }

        /// <summary>
        /// Asserts <see cref="NewUserRepository.FindById(Guid?, CancellationToken)"/>
        /// <see cref="User.UserId"> UserId of the user</see>
        /// should return a User when given valid userId which exits
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task FindById_ShouldReturnUser_WhenGivenValidUserId()
        {
            //Arrange
            AddUser();
            var cancellationToken = CancellationToken.None;

            //Act
            var user = await _newUserRepository.FindById(_userId, cancellationToken);

            //Assert
            Assert.NotNull(user);
            Assert.Equal(user.UserId, _userId);
        }

        /// <summary>
        /// Asserts <see cref="NewUserRepository.GetByEmailConfirmationSecretAndId(Guid?, string, CancellationToken)"/>
        /// <see cref="User.UserId"> UserId of the user</see> & 
        /// <see cref="User.EmailConfirmationSecret">User email confirmation secret</see>
        /// should return a User when given valid userId & emailconfirmation secret which exits
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task GetByEmailConfirmationSecretAndId_ShouldReturnUser_WhenGivenValidUserIdAndEmailConfirmationSecret()
        {
            //Arrange
            AddUser();
            var cancellationToken = CancellationToken.None;

            //Act
            var user = await _newUserRepository.GetByEmailConfirmationSecretAndId(_userId, _userEmailSecret, cancellationToken);

            //Assert
            Assert.NotNull(user);
            Assert.Equal(user.UserId, _userId);
            Assert.Equal(user.EmailConfirmationSecret, _userEmailSecret);
        }

        /// <summary>
        /// Asserts <see cref="NewUserRepository.Update(User)"/>
        /// should update user and return a User when given valid user
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task Update_ShouldUpdateUser_WhenGivenUser()
        {
            //Arrange
            AddUser();
            var user = await _newUserRepository.FindById(_userId);
            user.Firstname = "TestFirstname";
            user.Lastname = "TestLastName";

            //Act
            var userUpdate = _newUserRepository.Update(user);

            //Assert
            Assert.Equal(userUpdate.Firstname, user.Firstname);
            Assert.Equal(userUpdate.Lastname, user.Lastname);
        }

        /// <summary>
        /// Asserts <see cref="NewUserRepository.Add(User)"/>
        /// should Add new user and return a User when given valid user
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task Add_ShouldAddUser_WhenGivenUser()
        {
            //Arrange
            var user = new User() { Username = "TestUser" };

            //Act
            var userAdd = await _newUserRepository.Add(user);

            //Assert
            Assert.Equal(userAdd.Username, user.Username);
        }

        /// <summary>
        /// Asserts <see cref="NewUserRepository.Delete(User)"/>
        /// should mark Delete added user i.e. logical marking no physical deletion and return nothing
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task Delete_ShouldMarkDelete_WhenGivenUser()
        {
            //Arrange
            var user = new User() { Username = "TestUser" };
            var userAdd = await _newUserRepository.Add(user);

            //Act
            _newUserRepository.Delete(userAdd);
            var userResult = await _newUserRepository.FindById(userAdd.UserId);

            //Assert
            Assert.NotNull(userResult.DeleteDate);

        }
        internal void Dispose()
        {
            _efDbContext.Dispose();
        }

        private async void AddUser()
        {
            var country = new Country() { CountryId = 1 };
            var _softwareIdTwo = Guid.NewGuid();
            var userSoftwareOne = new UserSoftware()
            {
                UserSoftwareId = Guid.NewGuid(),
                UserId = _userId,
                SoftwareId = _softwareId,
                DeleteDate = null
            };
            var userSoftwareTwo = new UserSoftware()
            {
                UserSoftwareId = Guid.NewGuid(),
                UserId = _userId,
                SoftwareId = _softwareIdTwo,
                DeleteDate = DateTime.UtcNow
            };
            var userSoftwares = new List<UserSoftware> { userSoftwareOne, userSoftwareTwo };

            var organizaton = new Organisation
            {
                OrganisationId = _OrganisationId,
                Name = "TestOrganisation",
                BillingAddress = "TestAddress",
                BillingEmail = "TestEmail",
                VatNumber = "TestVat",
                Phone = "TestPhone"
            };

            var softwareOne = new Software() { SoftwareId = _softwareId };
            var softwareTwo = new Software() { SoftwareId = _softwareIdTwo };

            var user = new User()
            {
                UserId = _userId,
                Organisation = organizaton,
                Username = _userName,
                Email = _userEmail,
                EmailConfirmationSecret = _userEmailSecret,
                PasswordHash = "72t+3O3ArZoiCRXEAgbXtIlMYN3DUF51ThVb2ZJroY4=",
                PasswordSalt = "TestPassword",
                Firstname = "TestFirstName",
                Lastname = "TestLastName",
                Language = new Language() { LanguageId = 123 },
                UserSoftwares = userSoftwares,
                UserRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        UserId = _userId,
                        Role = new Role
                        {
                            Name = "Public",
                            SoftwareId = _softwareId,
                            Software = softwareOne
                        }
                    }
                }
            };

            await _efDbContext.AddAsync(softwareOne);
            await _efDbContext.AddAsync(softwareTwo);

            await _efDbContext.AddAsync(organizaton);
            await _efDbContext.AddAsync(user);

            await _efDbContext.SaveChangesAsync();
        }

    }
}
