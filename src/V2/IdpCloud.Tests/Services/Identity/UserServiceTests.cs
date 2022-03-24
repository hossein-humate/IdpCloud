using AutoMapper;
using IdpCloud.Common;
using IdpCloud.Common.Settings;
using IdpCloud.DataProvider.Entity.Enum;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.DataProvider.Entity.Security;
using IdpCloud.Sdk.Model;
using IdpCloud.Sdk.Model.Identity.Request.User;
using IdpCloud.ServiceProvider;
using IdpCloud.ServiceProvider.BackgroundWorker;
using IdpCloud.ServiceProvider.EntityService.Identity;
using IdpCloud.ServiceProvider.EntityService.Security;
using IdpCloud.ServiceProvider.InternalService.Mail;
using IdpCloud.ServiceProvider.Service.Identity;
using IdpCloud.ServiceProvider.Service.Security;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace IdpCloud.Tests.Services.Identity
{
    /// <summary>
    /// Test Related to <see cref="IUserService"/> functionalities
    /// </summary>
    public class UserServiceTests
    {
        private readonly IUserService _userService;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
        private readonly Mock<IBackgroundTaskQueue> _mockBackgroundTask = new();
        private readonly Mock<IEmailService> _mockMailService = new();
        private readonly Mock<IRateLimitService> _mockRateLimitService = new();
        private readonly Mock<IUserRepository> _mockUserRepository = new();
        private readonly Mock<IOrganisationRepository> _mockOrganisationRepository = new();
        private readonly Mock<IRoleRepository> _mockNewRoleRepository = new();
        private readonly Mock<IActivityRepository> _mockActivityRepository = new();
        private readonly IMapper _mapper;
        private readonly Mock<IRandomPasswordGenerator> _mockRandomPasswordGenerator = new();
        private readonly Mock<IResetPasswordRepository> _mockResetPasswordRepository = new();

        private readonly static RegisterRequest registerRequest = new()
        {
            Username = "TestUsername",
            Email = "Test@test.com",
            Password = "ComplexPass",
            Firstname = "TestFirstname",
            Lastname = "TestLastname",
            OrganisationName = "TestOrganisation",
            OrganisationBillingEmail = "TestOrganisationBillingEmail",
            OrganisationBillingAddress = "TestOrganisationBillingAddress",
            OrganisationVatNumber = "TestOrganisationVatNumber",
            OrganisationPhone = "TestOrganisationPhone",
        };
        private readonly static SendEmailConfirmationRequest SendEmailConfirmationRequest = new()
        {
            UsernameOrEmail = "TestUsername",
            Password = "TestPassword"
        };
        private const string clientIp = "TestClientIp";
        private const string clientAgent = "TestClientAgent";
        private Guid publicRole = Guid.NewGuid();

        private readonly Mock<IOptions<GlobalParameterSetting>> _mockGlobalParameterSetting = new();
        private readonly Mock<IOptions<MailServiceSetting>> _mockMailServiceSetting = new();
        /// <summary>
        /// Initialises an instance of <see cref="UserServiceTests"/>.
        /// </summary>
        public UserServiceTests()
        {
            _mockGlobalParameterSetting.Setup(x => x.Value).Returns(new GlobalParameterSetting { SsoPublicRoleId = publicRole });

            var config = new MapperConfiguration(cfg => cfg.AddMaps(Assembly.GetAssembly(typeof(REST.Startup))));
            var mapper = config.CreateMapper();
            _mapper = mapper;
            _userService = new UserService(
                _mockUnitOfWork.Object,
                _mockBackgroundTask.Object,
                _mockMailService.Object,
                _mockRateLimitService.Object,
                _mockUserRepository.Object,
                _mockNewRoleRepository.Object,
                _mockActivityRepository.Object,
                _mockGlobalParameterSetting.Object,
                _mockMailServiceSetting.Object,
                mapper,
                _mockRandomPasswordGenerator.Object,
                _mockResetPasswordRepository.Object);
        }

        /// <summary>
        /// Asserts that <see cref="UserService.Register(RegisterRequest, string, string, System.Threading.CancellationToken)"/>
        /// with a <see cref="RegisterRequest"/> that should has a valid username and Email address those are not used
        /// before for any other user
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task Register_ShouldReturnRequestSuccessful_WhenUsernameAndEmailAreValid()
        {
            //Arrange
            _mockRateLimitService
                 .Setup(m => m.CheckOnRateLimitAsync(It.IsAny<string>(),
                     It.IsAny<ActivityType>(),
                     It.IsAny<int>(),
                     It.IsAny<int>(),
                     It.IsAny<CancellationToken>()))
                 .ReturnsAsync(false);

            _mockUserRepository
               .Setup(x => x.GetByUsername(It.IsAny<string>(), default))
               .ReturnsAsync(default(User));

            _mockUserRepository
               .Setup(x => x.GetByEmail(It.IsAny<string>(), default))
               .ReturnsAsync(default(User));

            var organisation = new Organisation
            {
                Name = registerRequest.OrganisationName,
                BillingEmail = registerRequest.OrganisationBillingEmail,
                BillingAddress = registerRequest.OrganisationBillingAddress,
                Phone = registerRequest.OrganisationPhone,
                VatNumber = registerRequest.OrganisationVatNumber,
            };

            var user = new User
            {
                Organisation = organisation,
                Email = registerRequest.Email,
                Firstname = registerRequest.Firstname,
                Lastname = registerRequest.Lastname,
                Username = registerRequest.Username.Trim().ToLower(),
                UpdateDate = DateTime.UtcNow,
                RegisterIp = clientIp
            };
            _mockUserRepository.Setup(x => x.Add(It.IsAny<User>())).ReturnsAsync(user);

            var userRole = new UserRole
            {
                User = user,
                RoleId = publicRole
            };
            _mockNewRoleRepository.Setup(x => x.AddUserRole(It.IsAny<UserRole>())).ReturnsAsync(userRole);

            var activity = new Activity
            {
                Ip = clientIp,
                UserAgent = clientAgent,
                Type = ActivityType.EmailConfirmationOnRegister,
                UserId = user.UserId,
                User = user,
                Decription = $"Email confirmation link has been sent to {user.Email}"
            };
            _mockActivityRepository.Setup(x => x.AddAsync(It.IsAny<Activity>(), default)).ReturnsAsync(activity);
            _mockUnitOfWork.Setup(a => a.Activities).Returns(_mockActivityRepository.Object);
            _mockUnitOfWork.Setup(x => x.CompleteAsync(default)).ReturnsAsync(1);

            //Act
            var registerResult = await _userService.Register(registerRequest, clientIp, clientAgent, default);

            //Assert
            _mockRateLimitService.Verify(m => m.CheckOnRateLimitAsync(clientIp,
                ActivityType.EmailConfirmationOnRegister, 5, 24, CancellationToken.None), Times.Once());

            _mockUserRepository.Verify(x => x.GetByUsername(It.IsAny<string>(), default), Times.Once());

            _mockUserRepository.Verify(x => x.GetByEmail(It.IsAny<string>(), default), Times.Once());

            _mockOrganisationRepository.Verify(x => x.Add(It.IsAny<Organisation>()), Times.Never());

            _mockUserRepository.Verify(x => x.Add(It.Is<User>(u =>
                u.Email == registerRequest.Email && u.Username == registerRequest.Username.Trim().ToLower())), Times.Once());

            _mockNewRoleRepository.Verify(x => x.AddUserRole(It.Is<UserRole>(ur =>
               ur.User.Username == user.Username && ur.RoleId == publicRole)), Times.Once());

            _mockActivityRepository.Verify(x => x.AddAsync(It.Is<Activity>(a =>
              a.Type == ActivityType.EmailConfirmationOnRegister && a.Ip == clientIp), default), Times.Once());

            Assert.NotEqual(RequestResult.UsernameExist, registerResult);
            Assert.NotEqual(RequestResult.EmailExist, registerResult);
            Assert.Equal(RequestResult.RequestSuccessful, registerResult);
        }

        /// <summary>
        /// Asserts that <see cref="UserService.Register(RegisterRequest, string, string, System.Threading.CancellationToken)"/>
        /// with a <see cref="RegisterRequest"/> that should has a valid username, but the Email address 
        /// is aleardy exist in the database. The related User record should has expired Email Confirmation Link and 
        /// also has LoginTimes record equal 0 and EmailConfirmed equal false that represent this account never logged in before and is valid for this user
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task Register_ShouldReturnRequestSuccessful_WhenEmailIsReservedButConfirmLinkExpired()
        {
            //Arrange
            //this is related to the expired register request
            var Organisation = new Organisation
            {
                Name = registerRequest.OrganisationName,
                BillingEmail = registerRequest.OrganisationBillingEmail,
                BillingAddress = registerRequest.OrganisationBillingAddress,
                Phone = registerRequest.OrganisationPhone,
                VatNumber = registerRequest.OrganisationVatNumber,
            };

            var user = new User
            {
                Email = registerRequest.Email,
                UpdateDate = DateTime.UtcNow,
                EmailConfirmed = false,
                EmailConfirmationExpiry = DateTime.UtcNow.AddMinutes(-2),
                RegisterIp = clientIp
            };

            _mockUserRepository
               .Setup(x => x.GetByEmail(registerRequest.Email, default))
               .ReturnsAsync(user);

            var newUser = new User
            {
                Email = registerRequest.Email,
                Username = registerRequest.Username,
                UpdateDate = DateTime.UtcNow,
                RegisterIp = clientIp
            };
            _mockUserRepository.Setup(x => x.Add(newUser)).ReturnsAsync(newUser);

            var userRole = new UserRole
            {
                User = newUser,
                RoleId = publicRole
            };
            _mockNewRoleRepository.Setup(x => x.AddUserRole(It.IsAny<UserRole>())).ReturnsAsync(userRole);

            //Act
            var registerResult = await _userService.Register(registerRequest, clientIp, clientAgent, default);

            //Assert
            _mockUserRepository.Verify(x => x.GetByEmail(It.IsAny<string>(), default), Times.Once());

            _mockOrganisationRepository.Verify(x => x.Add(It.IsAny<Organisation>()), Times.Never());

            _mockUserRepository.Verify(x => x.Add(It.Is<User>(u =>
                u.Email == registerRequest.Email && u.Username == registerRequest.Username.ToLower())), Times.Once());

            _mockNewRoleRepository.Verify(x => x.AddUserRole(It.Is<UserRole>(ur =>
              ur.User.Username == newUser.Username.ToLower() && ur.RoleId == publicRole)), Times.Once());

            Assert.NotEqual(RequestResult.UsernameExist, registerResult);
            Assert.NotEqual(RequestResult.EmailExist, registerResult);
            Assert.Equal(RequestResult.RequestSuccessful, registerResult);
        }

        /// <summary>
        /// Asserts that <see cref="UserService.Register(RegisterRequest, string, string, System.Threading.CancellationToken)"/>
        /// with a <see cref="RegisterRequest"/> that should has invalid username
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task Register_ShouldReturnUsernameExist_WhenUsernameIsExistBefore()
        {
            //Arrange
            //this is related to the existing username 
            var user = new User
            {
                Email = registerRequest.Email,
                Username = registerRequest.Username,
                UpdateDate = DateTime.UtcNow,
                RegisterIp = clientIp
            };

            _mockUserRepository
               .Setup(x => x.GetByUsername(registerRequest.Username, default))
               .ReturnsAsync(user);

            //Act
            var registerResult = await _userService.Register(registerRequest, clientIp, clientAgent, default);

            //Assert
            _mockUserRepository.Verify(x => x.GetByUsername(registerRequest.Username, default), Times.Once());
            _mockOrganisationRepository.Verify(x => x.Add(It.IsAny<Organisation>()), Times.Never());
            _mockUserRepository.Verify(x => x.Add(It.IsAny<User>()), Times.Never());

            Assert.NotEqual(RequestResult.RequestSuccessful, registerResult);
            Assert.NotEqual(RequestResult.EmailExist, registerResult);
            Assert.Equal(RequestResult.UsernameExist, registerResult);
        }

        /// <summary>
        /// Asserts that <see cref="UserService.Register(RegisterRequest, string, string, System.Threading.CancellationToken)"/>
        /// with a <see cref="RegisterRequest"/> that should has same email address with ConfirmedEmail equal true
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task Register_ShouldReturnEmailExist_WhenGivenEmailIsExistAndConfirmedBefore()
        {
            //Arrange
            //this is related to the Confirmed Email Address
            var user = new User
            {
                Email = registerRequest.Email,
                Username = registerRequest.Username.Trim().ToLower(),
                UpdateDate = DateTime.UtcNow,
                EmailConfirmed = true,
                RegisterIp = clientIp
            };

            _mockUserRepository
            .Setup(x => x.GetByEmail(registerRequest.Email, default))
            .ReturnsAsync(user);

            //Act
            var registerResult = await _userService.Register(registerRequest, clientIp, clientAgent, default);

            //Assert
            _mockUserRepository.Verify(x => x.GetByEmail(registerRequest.Email, default), Times.Once());
            _mockOrganisationRepository.Verify(x => x.Add(It.IsAny<Organisation>()), Times.Never());
            _mockUserRepository.Verify(x => x.Add(It.IsAny<User>()), Times.Never());

            Assert.NotEqual(RequestResult.RequestSuccessful, registerResult);
            Assert.NotEqual(RequestResult.UsernameExist, registerResult);
            Assert.Equal(RequestResult.EmailExist, registerResult);
        }

        /// <summary>
        /// Asserts that <see cref="UserService.Register(RegisterRequest, string, string, System.Threading.CancellationToken)"/>
        /// with a <see cref="RegisterRequest"/> that should has same email address in database with 
        /// Successful login times 
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task Register_ShouldReturnEmailExist_WhenGivenEmailIsExistAndHasSuccessfulLoggedInBefore()
        {
            //Arrange
            //this is related to Successful Logged In account
            var user = new User
            {
                Email = registerRequest.Email,
                Username = registerRequest.Username.Trim().ToLower(),
                UpdateDate = DateTime.UtcNow,
                LoginTimes = 1,
                RegisterIp = clientIp
            };

            _mockUserRepository
            .Setup(x => x.GetByEmail(registerRequest.Email, default))
            .ReturnsAsync(user);

            //Act
            var registerResult = await _userService.Register(registerRequest, clientIp, clientAgent, default);

            //Assert
            _mockUserRepository.Verify(x => x.GetByEmail(registerRequest.Email, default), Times.Once());
            _mockOrganisationRepository.Verify(x => x.Add(It.IsAny<Organisation>()), Times.Never());
            _mockUserRepository.Verify(x => x.Add(It.IsAny<User>()), Times.Never());

            Assert.NotEqual(RequestResult.RequestSuccessful, registerResult);
            Assert.NotEqual(RequestResult.UsernameExist, registerResult);
            Assert.Equal(RequestResult.EmailExist, registerResult);
        }

        /// <summary>
        /// Asserts that <see cref="UserService.Register(RegisterRequest, string, string, System.Threading.CancellationToken)"/>
        /// with a <see cref="RegisterRequest"/> that should has same email address in database with 
        /// Valid Email confirmation link(not expire yet)
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task Register_ShouldReturnEmailExist_WhenGivenEmailIsExistAndConfirmLinkNotExpire()
        {
            //Arrange
            //this is related to the Not Expired Link  
            var user = new User
            {
                Email = registerRequest.Email,
                Username = registerRequest.Username.Trim().ToLower(),
                UpdateDate = DateTime.UtcNow,
                EmailConfirmed = false,
                EmailConfirmationExpiry = DateTime.UtcNow.AddMinutes(5),
                RegisterIp = clientIp
            };

            _mockUserRepository
            .Setup(x => x.GetByEmail(registerRequest.Email, default))
            .ReturnsAsync(user);

            //Act
            var registerResult = await _userService.Register(registerRequest, clientIp, clientAgent, default);

            //Assert
            _mockUserRepository.Verify(x => x.GetByEmail(registerRequest.Email, default), Times.Once());
            _mockOrganisationRepository.Verify(x => x.Add(It.IsAny<Organisation>()), Times.Never());
            _mockUserRepository.Verify(x => x.Add(It.IsAny<User>()), Times.Never());

            Assert.NotEqual(RequestResult.RequestSuccessful, registerResult);
            Assert.NotEqual(RequestResult.UsernameExist, registerResult);
            Assert.Equal(RequestResult.EmailExist, registerResult);
        }

        /// <summary>
        /// Asserts that <see cref="UserService.Register(RegisterRequest, string, string, System.Threading.CancellationToken)"/>
        /// with a <see cref="RegisterRequest"/> that came from same IP Address more than 5 times
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task Register_ShouldReturnRequestSuccessful_WhenMoreThan5RegisterRequestCameFromSameIp()
        {
            //Arrange     
            _mockRateLimitService
              .Setup(m => m.CheckOnRateLimitAsync(clientIp, ActivityType.EmailConfirmationOnRegister, 5, 24, default))
              .ReturnsAsync(true);

            //Act
            var registerResult = await _userService.Register(registerRequest, clientIp, clientAgent, default);

            //Assert
            _mockRateLimitService.Verify(m => m.CheckOnRateLimitAsync(clientIp,
                ActivityType.EmailConfirmationOnRegister, 5, 24, CancellationToken.None), Times.Once());
            _mockOrganisationRepository.Verify(x => x.Add(It.IsAny<Organisation>()), Times.Never());
            _mockUserRepository.Verify(x => x.Add(It.IsAny<User>()), Times.Never());

            Assert.NotEqual(RequestResult.EmailExist, registerResult);
            Assert.NotEqual(RequestResult.UsernameExist, registerResult);
            Assert.Equal(RequestResult.RequestSuccessful, registerResult);
        }

        /// <summary>
        /// Asserts that <see cref="UserService.SendEmailConfirmation(SendEmailConfirmationRequest, string, string, CancellationToken)"/>
        /// with a <see cref="SendEmailConfirmationRequest"/> that should has a valid username or Email address and Password
        /// and also not confirmed email address yet
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task SendEmailConfirmation_ShouldReturnRequestSuccessful_WhenValidCredentialAndEmailNotConfirmed()
        {
            //Arrange
            _mockRateLimitService
                 .Setup(m => m.CheckOnRateLimitAsync(It.IsAny<string>(),
                     It.IsAny<ActivityType>(),
                     It.IsAny<int>(),
                     It.IsAny<int>(),
                     It.IsAny<CancellationToken>()))
                 .ReturnsAsync(false);

            var user = new User
            {
                Username = SendEmailConfirmationRequest.UsernameOrEmail,
                EmailConfirmed = false,
                EmailConfirmationExpiry = DateTime.UtcNow.AddDays(3),
                EmailConfirmationSecret = Cryptography.GenerateSecret(32)
            };

            _mockUserRepository
               .Setup(x => x.GetUserByUserNameOrEmailAndPassword(It.IsAny<string>(), It.IsAny<string>(), default))
               .ReturnsAsync(user);

            _mockUserRepository.Setup(x => x.Update(It.IsAny<User>())).Returns(user);

            var activity = new Activity
            {
                Ip = clientIp,
                UserAgent = clientAgent,
                Type = ActivityType.ResendEmailConfirmationLink,
                UserId = user.UserId,
                User = user,
            };
            _mockActivityRepository.Setup(x => x.AddAsync(It.IsAny<Activity>(), default)).ReturnsAsync(activity);
            _mockUnitOfWork.Setup(a => a.Activities).Returns(_mockActivityRepository.Object);
            _mockUnitOfWork.Setup(x => x.CompleteAsync(default)).ReturnsAsync(1);

            //Act
            var result = await _userService.SendEmailConfirmation(SendEmailConfirmationRequest, clientIp, clientAgent, default);

            //Assert
            _mockRateLimitService.Verify(m => m.CheckOnRateLimitAsync(clientIp,
                ActivityType.ResendEmailConfirmationLink, 5, 24, CancellationToken.None), Times.Once());

            _mockUserRepository.Verify(x => x.GetUserByUserNameOrEmailAndPassword(
                SendEmailConfirmationRequest.UsernameOrEmail, SendEmailConfirmationRequest.Password, default), Times.Once());

            _mockUserRepository.Verify(x => x.Update(It.Is<User>(u =>
                u.Username == SendEmailConfirmationRequest.UsernameOrEmail)), Times.Once());

            _mockActivityRepository.Verify(x => x.AddAsync(It.Is<Activity>(a =>
              a.Type == ActivityType.ResendEmailConfirmationLink && a.Ip == clientIp), default), Times.Once());

            Assert.NotEqual(RequestResult.EmailAddressConfirmed, result);
            Assert.NotEqual(RequestResult.EmailUsernameOrPasswordWrong, result);
            Assert.Equal(RequestResult.RequestSuccessful, result);
        }

        /// <summary>
        /// Asserts that <see cref="UserService.SendEmailConfirmation(SendEmailConfirmationRequest, string, string, CancellationToken)"/>
        /// with a <see cref="SendEmailConfirmationRequest"/> that should has a valid username or Email address and Password
        /// and also user has confirmed email address before
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task SendEmailConfirmation_ShouldReturnEmailAddressConfirmed_WhenValidCredentialAndEmailConfirmed()
        {
            //Arrange
            _mockRateLimitService
                 .Setup(m => m.CheckOnRateLimitAsync(It.IsAny<string>(),
                     It.IsAny<ActivityType>(),
                     It.IsAny<int>(),
                     It.IsAny<int>(),
                     It.IsAny<CancellationToken>()))
                 .ReturnsAsync(false);

            var user = new User
            {
                Username = SendEmailConfirmationRequest.UsernameOrEmail,
                EmailConfirmed = true,
                EmailConfirmationExpiry = DateTime.UtcNow.AddDays(3),
                EmailConfirmationSecret = Cryptography.GenerateSecret(32)
            };

            _mockUserRepository
               .Setup(x => x.GetUserByUserNameOrEmailAndPassword(It.IsAny<string>(), It.IsAny<string>(), default))
               .ReturnsAsync(user);

            //Act
            var result = await _userService.SendEmailConfirmation(SendEmailConfirmationRequest, clientIp, clientAgent, default);

            //Assert
            _mockRateLimitService.Verify(m => m.CheckOnRateLimitAsync(clientIp,
                ActivityType.ResendEmailConfirmationLink, 5, 24, CancellationToken.None), Times.Once());

            _mockUserRepository.Verify(x => x.GetUserByUserNameOrEmailAndPassword(
                SendEmailConfirmationRequest.UsernameOrEmail, SendEmailConfirmationRequest.Password, default), Times.Once());

            Assert.NotEqual(RequestResult.RequestSuccessful, result);
            Assert.NotEqual(RequestResult.EmailUsernameOrPasswordWrong, result);
            Assert.Equal(RequestResult.EmailAddressConfirmed, result);
        }

        /// <summary>
        /// Asserts that <see cref="UserService.SendEmailConfirmation(SendEmailConfirmationRequest, string, string, CancellationToken)"/>
        /// with a <see cref="SendEmailConfirmationRequest"/> that should has a invalid username or Email address and Password
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task SendEmailConfirmation_ShouldReturnEmailUsernameOrPasswordWrong_WhenInvalidCredential()
        {
            //Arrange
            _mockRateLimitService
                 .Setup(m => m.CheckOnRateLimitAsync(It.IsAny<string>(),
                     It.IsAny<ActivityType>(),
                     It.IsAny<int>(),
                     It.IsAny<int>(),
                     It.IsAny<CancellationToken>()))
                 .ReturnsAsync(false);

            _mockUserRepository
               .Setup(x => x.GetUserByUserNameOrEmailAndPassword(It.IsAny<string>(), It.IsAny<string>(), default))
               .ReturnsAsync(default(User));

            //Act
            var result = await _userService.SendEmailConfirmation(SendEmailConfirmationRequest, clientIp, clientAgent, default);

            //Assert
            _mockRateLimitService.Verify(m => m.CheckOnRateLimitAsync(clientIp,
                ActivityType.ResendEmailConfirmationLink, 5, 24, CancellationToken.None), Times.Once());

            _mockUserRepository.Verify(x => x.GetUserByUserNameOrEmailAndPassword(
                SendEmailConfirmationRequest.UsernameOrEmail, SendEmailConfirmationRequest.Password, default), Times.Once());

            Assert.NotEqual(RequestResult.RequestSuccessful, result);
            Assert.NotEqual(RequestResult.EmailAddressConfirmed, result);
            Assert.Equal(RequestResult.EmailUsernameOrPasswordWrong, result);
        }

        /// <summary>
        /// Asserts that <see cref="UserService.SendEmailConfirmation(SendEmailConfirmationRequest, string, string, CancellationToken)"/>
        /// with a <see cref="SendEmailConfirmationRequest"/> that came from same IP Address more than 5 times
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task SendEmailConfirmation_ShouldReturnRequestSuccessful_WhenMoreThan5RegisterRequestCameFromSameIp()
        {
            //Arrange     
            _mockRateLimitService
              .Setup(m => m.CheckOnRateLimitAsync(clientIp, ActivityType.ResendEmailConfirmationLink, 5, 24, default))
              .ReturnsAsync(true);

            //Act
            var result = await _userService.SendEmailConfirmation(SendEmailConfirmationRequest, clientIp, clientAgent, default);

            //Assert
            _mockRateLimitService.Verify(m => m.CheckOnRateLimitAsync(clientIp,
                ActivityType.ResendEmailConfirmationLink, 5, 24, CancellationToken.None), Times.Once());

            Assert.NotEqual(RequestResult.EmailExist, result);
            Assert.NotEqual(RequestResult.UsernameExist, result);
            Assert.Equal(RequestResult.RequestSuccessful, result);
        }

        /// <summary>
        /// Asserts that <see cref="UserService.ConfirmEmail(string, string, CancellationToken)"/>
        /// with empty key and secret should  return
        /// <see cref="RequestResult">RequestResult.EmptyEntryNotAllowed</see>
        /// and do not update user for Email confirm and should not send Email.
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task ConfirmEmail_ShouldReturnRequestResultEmptyEntryNotAllowed_WhenKeyOrSecretIsNull()
        {
            //Arrange
            var cancelaltionToken = CancellationToken.None;

            var key = string.Empty;
            var secret = string.Empty;

            //Act
            var registerResult = await _userService.ConfirmEmail(key, secret, cancelaltionToken);

            //Assert
            Assert.Equal(RequestResult.EmptyEntryNotAllowed, registerResult);
        }

        /// <summary>
        /// Asserts that <see cref="UserService.ConfirmEmail(string, string, CancellationToken)"/>
        /// with valid key and secret should update the user with EmailConfirmed true and returns 
        /// <see cref="RequestResult">RequestResult.RequestSuccessful</see>
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task ConfirmEmail_ShouldReturnNotAllowedThisOperation_WhenKeyAndSecretIsValid()
        {
            //Arrange
            var cancelaltionToken = CancellationToken.None;
            var key = "RkRFQTM4N0ItMDQzQS00NkIzLTk2MUYtODI5Qjc1QzYwNURC";
            var secret = "B7UUy8eRNtlVAhQ2N8LKr20rlthcJTPSU6XQPtuiLVw=";

            var user = new User() { UserId = Guid.NewGuid(), EmailConfirmed = false, EmailConfirmationExpiry = DateTime.UtcNow.AddDays(2) };

            _mockUserRepository
               .Setup(x => x.GetByEmailConfirmationSecretAndId(
                   It.IsAny<Guid>(),
                   It.IsAny<string>(),
                   It.IsAny<CancellationToken>()))
               .ReturnsAsync(user);

            _mockUserRepository
                .Setup(x => x.Update(It.IsAny<User>()))
                .Returns(user);

            _mockUnitOfWork
                .Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            //Act
            var registerResult = await _userService.ConfirmEmail(key, secret, cancelaltionToken);

            //Assert
            Assert.Equal(RequestResult.RequestSuccessful, registerResult);

            _mockUserRepository.Verify(x => x.GetByEmailConfirmationSecretAndId(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                default), Times.Once());

            _mockUserRepository.Verify(x => x.Update(user), Times.Once());

            _mockUnitOfWork.Verify(x => x.CompleteAsync(cancelaltionToken), Times.Once());

            _mockBackgroundTask.Verify(m => m.QueueSendMail(It.IsAny<Func<CancellationToken, Task>>()), Times.Once());
        }

        /// <summary>
        /// Asserts that <see cref="UserService.ConfirmEmail(string, string, CancellationToken)"/>
        /// with key and secret should return
        /// <see cref="RequestResult">RequestResult.NotAllowedThisOperation</see>
        /// </summary> When user not found or invalid user also should not update EmailoConfirmed flag and do not send email
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task ConfirmEmail_ShouldReturnRequestNotAllowedThisOperation_WhenUserNotFound()
        {
            //Arrange
            var cancelaltionToken = CancellationToken.None;
            var key = "RkRFQTM4N0ItMDQzQS00NkIzLTk2MUYtODI5Qjc1QzYwNURC";
            var secret = "B7UUy8eRNtlVAhQ2N8LKr20rlthcJTPSU6XQPtuiLVw=";

            _mockUserRepository
               .Setup(x => x.GetByEmailConfirmationSecretAndId(
                   It.IsAny<Guid>(),
                   It.IsAny<string>(),
                   It.IsAny<CancellationToken>()))
               .ReturnsAsync(null as User);

            //Act
            var registerResult = await _userService.ConfirmEmail(key, secret, cancelaltionToken);

            //Assert
            Assert.Equal(RequestResult.NotAllowedThisOperation, registerResult);

            _mockUserRepository.Verify(x => x.GetByEmailConfirmationSecretAndId(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                default), Times.Once());

            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Never());

            _mockUnitOfWork.Verify(x => x.CompleteAsync(cancelaltionToken), Times.Never());

            _mockBackgroundTask.Verify(m => m.QueueSendMail(It.IsAny<Func<CancellationToken, Task>>()), Times.Never());
        }

        /// <summary>
        /// Asserts that <see cref="UserService.ConfirmEmail(string, string, CancellationToken)"/>
        /// with key and secret should return
        /// <see cref="RequestResult">RequestResult.ConfirmationCompletedBefore</see>
        /// </summary> When email already confirmed and do nothing
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task ConfirmEmail_ShouldReturnRequestConfirmationCompletedBefore_WhenEmailConfirmationAlreadyDone()
        {
            //Arrange
            var cancelaltionToken = CancellationToken.None;
            var key = "RkRFQTM4N0ItMDQzQS00NkIzLTk2MUYtODI5Qjc1QzYwNURC";
            var secret = "B7UUy8eRNtlVAhQ2N8LKr20rlthcJTPSU6XQPtuiLVw=";

            var user = new User() { UserId = Guid.NewGuid(), EmailConfirmed = true };
            _mockUserRepository
               .Setup(x => x.GetByEmailConfirmationSecretAndId(
                   It.IsAny<Guid>(),
                   It.IsAny<string>(),
                   It.IsAny<CancellationToken>()))
               .ReturnsAsync(user);

            //Act
            var registerResult = await _userService.ConfirmEmail(key, secret, cancelaltionToken);

            //Assert
            Assert.Equal(RequestResult.ConfirmationCompletedBefore, registerResult);

            _mockUserRepository.Verify(x => x.GetByEmailConfirmationSecretAndId(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                default), Times.Once());

            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Never());

            _mockUnitOfWork.Verify(x => x.CompleteAsync(cancelaltionToken), Times.Never());

            _mockBackgroundTask.Verify(m => m.QueueSendMail(It.IsAny<Func<CancellationToken, Task>>()), Times.Never());
        }

        /// <summary>
        /// Asserts that <see cref="UserService.ConfirmEmail(string, string, CancellationToken)"/>
        /// with key and secret should return
        /// <see cref="RequestResult">RequestResult.ConfirmationLinkExpired</see>
        /// </summary> When email confirmation link is Expired and do nothing
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task ConfirmEmail_ShouldReturnRequestConfirmationLinkExpired_WhenEmailConfirmationLinkIsExpired()
        {
            //Arrange
            var cancelaltionToken = CancellationToken.None;

            var key = "RkRFQTM4N0ItMDQzQS00NkIzLTk2MUYtODI5Qjc1QzYwNURC";
            var secret = "B7UUy8eRNtlVAhQ2N8LKr20rlthcJTPSU6XQPtuiLVw=";

            var user = new User() { UserId = Guid.NewGuid(), EmailConfirmed = false, EmailConfirmationExpiry = DateTime.UtcNow.AddMinutes(-5) };

            _mockUserRepository
               .Setup(x => x.GetByEmailConfirmationSecretAndId(
                   It.IsAny<Guid>(),
                   It.IsAny<string>(),
                   It.IsAny<CancellationToken>()))
               .ReturnsAsync(user);

            //Act
            var registerResult = await _userService.ConfirmEmail(key, secret, cancelaltionToken);

            //Assert
            Assert.Equal(RequestResult.ConfirmationLinkExpired, registerResult);

            _mockUserRepository.Verify(x => x.GetByEmailConfirmationSecretAndId(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                default), Times.Once());

            _mockUserRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Never());

            _mockUnitOfWork.Verify(x => x.CompleteAsync(cancelaltionToken), Times.Never());

            _mockBackgroundTask.Verify(m => m.QueueSendMail(It.IsAny<Func<CancellationToken, Task>>()), Times.Never());
        }

        /// <summary>
        /// Asserts when calling <see cref="UserService.Create(CreateUserRequest, CancellationToken)"/>
        /// will populate map DTO to user entity and Add it in the database
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task Create_ShouldCreateAndReturnNewUser_WhenGivenCreateRequest()
        {
            //Arrange
            var roleId = Guid.NewGuid();
            var request = new CreateUserRequest
            {
                Firstname = "Test",
                Lastname = "TestLastName",
                Username = "TestUserName",
                Email = "Test@Test.com",
                OrganisationId = 1,
                Mobile = "1234567891",
                RoleId = roleId.ToString()
            };

            var user = _mapper.Map<User>(request);

            var userRole = new UserRole() { User = user, RoleId = roleId };

            var resetPassword = new ResetPassword() { Secret = "TestSecret" };

            _mockRandomPasswordGenerator
                .Setup(x => x.RandomPassword())
                .Returns("TestPassword");

            _mockUserRepository
                .Setup(m => m.Add(It.IsAny<User>()))
                .ReturnsAsync(user);

            _mockNewRoleRepository
                .Setup(m => m.AddUserRole(It.IsAny<UserRole>()))
                .ReturnsAsync(userRole);

            _mockUnitOfWork
              .Setup(m => m.CompleteAsync(default))
              .ReturnsAsync(1);

            _mockResetPasswordRepository
                .Setup(m => m.AddAsync(It.IsAny<ResetPassword>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(resetPassword);

            //Act
            var createUser = await _userService.Create(request);

            //Assert
            _mockUserRepository.Verify(m => m.Add(It.Is<User>(u =>
                u.Firstname == request.Firstname
                && u.Lastname == request.Lastname
                && u.Username == request.Username
                && u.OrganisationId == request.OrganisationId
                && u.Mobile == request.Mobile
                && u.PasswordHash != null
                && u.PasswordSalt != null
               )), Times.Once());

            _mockRandomPasswordGenerator.Verify(x => x.RandomPassword(), Times.Once());

            _mockNewRoleRepository.Verify(m => m.AddUserRole(It.Is<UserRole>(ur => ur.RoleId == roleId)), Times.Once());

            _mockUnitOfWork.Verify(m => m.CompleteAsync(default), Times.Once());

            _mockResetPasswordRepository.Verify(m => m.AddAsync(It.IsAny<ResetPassword>(), It.IsAny<CancellationToken>()), Times.Once());

            Assert.Equal(user.Firstname, request.Firstname);
            Assert.Equal(user.Lastname, request.Lastname);
            Assert.Equal(user.Email, request.Email);
            Assert.Equal(user.OrganisationId, request.OrganisationId);
            Assert.Equal(user.Mobile, request.Mobile);

            Assert.Equal(user.Firstname, createUser.Firstname);
            Assert.Equal(user.Lastname, createUser.Lastname);
            Assert.Equal(user.Email, createUser.Email);
            Assert.Equal(user.OrganisationId, createUser.OrganisationId);
            Assert.Equal(user.Mobile, createUser.Mobile);

        }

        /// <summary>
        /// Asserts when calling <see cref="UserService.Update(UpdateUserRequest, CancellationToken)"/>
        /// will update the user DTO it in the database
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task Update_ShouldUpdateAndReturnUpdatedUser_WhenGivenUpdateRequest()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();

            var request = new UpdateUserRequest
            {
                UserId = userId,
                Firstname = "TestFirstName",
                Lastname = "TestLastName",
                Email = "Test@Test.com",
                Mobile = "123456758",
                Username = "TestUserNameupdate",
                OrganisationId = 1,
                RoleId = roleId
            };

            var user = _mapper.Map<User>(request);

            var userRole = new UserRole { UserRoleId = Guid.NewGuid(), UserId = userId, RoleId = roleId };

            _mockUserRepository
                .Setup(m => m.FindById(It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mockUserRepository
               .Setup(m => m.Update(It.IsAny<User>()))
               .Returns(user);

            _mockNewRoleRepository
                .Setup(r => r.FindByUserId(It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(userRole);

            _mockUnitOfWork.Setup(x => x.CompleteAsync(default));

            //Act
            var result = _userService.Update(request).Result;

            //Assert
            _mockUserRepository.Verify(m => m.FindById(user.UserId, default), Times.Once);

            _mockUserRepository.Verify(m => m.Update(It.Is<User>(u =>
            u.Firstname == request.Firstname &&
            u.Lastname == request.Lastname &&
            u.Email == request.Email &&
            u.OrganisationId == request.OrganisationId &&
            u.Mobile == request.Mobile
            )), Times.Once());

            _mockNewRoleRepository.Verify(m => m.FindByUserId(userId, default), Times.Once());

            _mockNewRoleRepository.Verify(m => m.AddUserRole(It.IsAny<UserRole>()), Times.Never);

            Assert.Equal(user.UserId, result.UserId);
            Assert.Equal(user.Username, result.Username);
            Assert.Equal(user.Firstname, result.Firstname);
            Assert.Equal(user.Lastname, result.Lastname);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.OrganisationId, result.OrganisationId);
        }

        /// <summary>
        /// Asserts when calling <see cref="UserService.Update(UpdateUserRequest, CancellationToken)"/>
        /// will update the user DTO it in the database and update and add userrole in UserRoles table
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task Update_ShouldUpdateAndReturnUpdatedUser_AndAddUserRoleWhenRoleChanges_AndMarkDeleteExisting_WhenGivenUpdateRequest()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();

            var request = new UpdateUserRequest
            {
                UserId = userId,
                Firstname = "TestFirstName",
                Lastname = "TestLastName",
                Email = "Test@Test.com",
                Mobile = "123456758",
                Username = "TestUserName",
                OrganisationId = 1,
                RoleId = roleId
            };

            var user = _mapper.Map<User>(request);

            var userRole = new UserRole { UserId = userId, RoleId = Guid.NewGuid(), UserRoleId = Guid.NewGuid() };

            var existingRoleId = Guid.NewGuid();

            var existingUserRole = new UserRole { UserId = userId, RoleId = existingRoleId, UserRoleId = Guid.NewGuid() };

            _mockUserRepository
               .Setup(m => m.FindById(It.IsAny<Guid>(),
               It.IsAny<CancellationToken>()))
               .ReturnsAsync(user);

            _mockUserRepository
               .Setup(m => m.Update(It.IsAny<User>()))
               .Returns(user);

            _mockNewRoleRepository
                .Setup(r => r.FindByUserId(It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUserRole);


            _mockNewRoleRepository
               .Setup(r => r.AddUserRole(It.IsAny<UserRole>()))
               .ReturnsAsync(userRole);

            _mockUnitOfWork.Setup(x => x.CompleteAsync(default));

            //Act
            var result = _userService.Update(request).Result;

            //Assert

            _mockUserRepository.Verify(m => m.FindById(user.UserId, default), Times.Once);

            _mockUserRepository.Verify(m => m.Update(It.Is<User>(u =>
            u.Firstname == request.Firstname &&
            u.Lastname == request.Lastname &&
            u.Email == request.Email &&
            u.OrganisationId == request.OrganisationId &&
            u.Mobile == request.Mobile
            )), Times.Once());

            _mockNewRoleRepository.Verify(m => m.FindByUserId(userId, default), Times.Once());

            _mockNewRoleRepository.Verify(m => m.AddUserRole(It.Is<UserRole>(u =>
            u.User.UserId == userId &&
            u.RoleId == roleId && u.DeleteDate == null)), Times.Once);

            _mockNewRoleRepository.Verify(m => m.DeleteUserRole(It.IsAny<UserRole>()), Times.Once);

            Assert.Equal(user.UserId, result.UserId);
            Assert.Equal(user.Username, result.Username);
            Assert.Equal(user.Firstname, result.Firstname);
            Assert.Equal(user.Lastname, result.Lastname);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.OrganisationId, result.OrganisationId);
        }
    }
}
