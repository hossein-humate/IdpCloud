using IdpCloud.Common;
using IdpCloud.Common.Settings;
using IdpCloud.DataProvider.Entity.Enum;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.DataProvider.Entity.Security;
using IdpCloud.DataProvider.Entity.SSO;
using IdpCloud.Sdk.Model.Security.Request.ResetPassword;
using IdpCloud.ServiceProvider;
using IdpCloud.ServiceProvider.BackgroundWorker;
using IdpCloud.ServiceProvider.EntityService.Identity;
using IdpCloud.ServiceProvider.EntityService.Security;
using IdpCloud.ServiceProvider.InternalService.Mail;
using IdpCloud.ServiceProvider.Service;
using IdpCloud.ServiceProvider.Service.Security;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace IdpCloud.Tests.Services.Security
{
    /// <summary>
    /// Test suite for <see cref="ResetPasswordService"/>.
    /// </summary>
    public class ResetPasswordServiceTests
    {
        private readonly IResetPasswordService _resetPasswordService;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
        private readonly Mock<IBackgroundTaskQueue> _mockBackgroundTaskQueue = new();
        private readonly Mock<IEmailService> _mockMailService = new();
        private readonly Mock<IRateLimitService> _mockRateLimitService = new();
        private readonly Mock<IUserRepository> _mockUserRepository = new();
        private readonly Mock<IActivityRepository> _mockActivityRepository = new();
        private readonly Mock<ICurrentUserSessionService> _mockCurrentUserSessionService = new();
        private readonly Mock<IOptions<GlobalParameterSetting>> _mockGlobalParameterSetting = new();
        /// <summary>
        /// Initialises an instance of <see cref="ResetPasswordServiceTests"/>.
        /// </summary>
        public ResetPasswordServiceTests()
        {
            _resetPasswordService = new ResetPasswordService(_mockUnitOfWork.Object,
                _mockBackgroundTaskQueue.Object,
                _mockMailService.Object,
                _mockRateLimitService.Object,
                _mockUserRepository.Object,
                _mockActivityRepository.Object,
                _mockCurrentUserSessionService.Object,
               _mockGlobalParameterSetting.Object
                );
        }

        /// <summary>
        /// Asserts that <see cref="ResetPasswordService.RequestByEmailAsync(string, string, string, CancellationToken)"/>
        /// should verify User for given email and Add ResetPassword and ActivityType of ActivityType.RequestResetPasswordByEmail
        /// and send email to user
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task RequestByEmailAsync()
        {
            //Arrange
            string email = "TestEmail";
            string clientIp = "TestClientIp";
            string clientUserAgent = "TestClientUserAgent";
            var cancellationToken = CancellationToken.None;

            var mockResetPasswordRepository = new Mock<IResetPasswordRepository>();

            _mockRateLimitService
                .Setup(m => m.CheckOnRateLimitAsync(It.IsAny<string>(),
                    It.IsAny<ActivityType>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var userId = Guid.NewGuid();
            var user = new User();

            var resetPasswordId = Guid.NewGuid();
            var resetPasssword = new ResetPassword()
            {
                ResetPasswordId = resetPasswordId,
                Secret = Cryptography.GenerateSecret(32),
                Expiry = DateTime.UtcNow.AddHours(1),
                User = user,
            };

            var activityId = Guid.NewGuid();
            var activity = new Activity() { ActivityId = activityId };

            _mockUserRepository
                .Setup(u => u.GetByEmail(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            mockResetPasswordRepository
                .Setup(rp => rp.AddAsync(It.IsAny<ResetPassword>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(resetPasssword);

            _mockUnitOfWork
                .Setup(u => u.ResetPasswords)
                .Returns(mockResetPasswordRepository.Object);

            _mockActivityRepository
                .Setup(a => a.AddAsync(It.IsAny<Activity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(activity);

            _mockUnitOfWork
                .Setup(m => m.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            //Act
            await _resetPasswordService.RequestByEmailAsync(email, clientIp, clientUserAgent, cancellationToken);

            //Assert
            _mockRateLimitService.Verify(m => m.CheckOnRateLimitAsync(clientIp, ActivityType.RequestResetPasswordByEmail, 5, 24, cancellationToken), Times.Once());

            _mockUserRepository.Verify(u => u.GetByEmail(email, cancellationToken), Times.Once());

            mockResetPasswordRepository.Verify(m => m.AddAsync(
                It.Is<ResetPassword>(rp => rp.User == user),
               cancellationToken), Times.Once());

            _mockUnitOfWork.Verify(m => m.ResetPasswords, Times.Once());

            _mockActivityRepository.Verify(m => m.AddAsync(
                It.Is<Activity>(a => a.UserAgent == clientUserAgent && a.Ip == clientIp && a.ResetPassword != null),
                It.IsAny<CancellationToken>()), Times.Once());

            _mockUnitOfWork.Verify(m => m.CompleteAsync(default), Times.Once());

            _mockBackgroundTaskQueue.Verify(m => m.QueueSendMail(It.IsAny<Func<CancellationToken, Task>>()), Times.Once());

        }

        /// <summary>
        /// Asserts that <see cref="ResetPasswordService.RequestByEmailAsync(string, string, string, CancellationToken)"/>
        /// should return and do nothing when CheckOnRateLimitAsync Returns True
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task RequestByEmailAsync_ShouldDoNothingAndReturn_WhenCheckOnRateLimitAsyncReturnTrue()
        {

            //Arrange
            string email = "TestEmail";
            string clientIp = "TestClientIp";
            string clientUserAgent = "TestClientUserAgent";
            var cancellationToken = CancellationToken.None;
            var activityType = ActivityType.RequestResetPasswordByEmail;

            _mockRateLimitService
                .Setup(m => m.CheckOnRateLimitAsync(It.IsAny<string>(),
                    It.IsAny<ActivityType>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            //Act
            await _resetPasswordService.RequestByEmailAsync(email, clientIp, clientUserAgent, cancellationToken);

            //Assert
            _mockRateLimitService.Verify(m => m.CheckOnRateLimitAsync(clientIp, activityType, 5, 24, cancellationToken), Times.Once());
            _mockUserRepository.Verify(m => m.GetByEmail(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());
            _mockUnitOfWork.Verify(m => m.Users, Times.Never());
            _mockUnitOfWork.Verify(m => m.ResetPasswords, Times.Never());
            _mockUnitOfWork.Verify(m => m.Activities, Times.Never());
            _mockUnitOfWork.Verify(m => m.CompleteAsync(cancellationToken), Times.Never());
            _mockBackgroundTaskQueue.Verify(m => m.QueueSendMail(It.IsAny<Func<CancellationToken, Task>>()), Times.Never());

        }

        /// <summary>
        /// Asserts that <see cref="ResetPasswordService.RequestByEmailAsync(string, string, string, CancellationToken)"/>
        /// should add Activity for Email nto found when user not found and send an email 
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task RequestByEmailAsync_ShouldAddActivityAndDoNotSendEmail_WhenUserNotFound()
        {
            //Arrange
            string email = "TestEmail";
            string clientIp = "TestClientIp";
            string clientUserAgent = "TestClientUserAgent";
            var cancellationToken = CancellationToken.None;

            var mockResetPasswordRepository = new Mock<IResetPasswordRepository>();

            _mockRateLimitService
                .Setup(m => m.CheckOnRateLimitAsync(It.IsAny<string>(),
                    It.IsAny<ActivityType>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var userId = Guid.NewGuid();
            var user = new User();

            var activityId = Guid.NewGuid();
            var activity = new Activity() { ActivityId = activityId };

            _mockUserRepository
                .Setup(u => u.GetByEmail(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as User);

            _mockActivityRepository
                .Setup(a => a.AddAsync(It.IsAny<Activity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(activity);

            _mockUnitOfWork
                .Setup(a => a.Activities)
                .Returns(_mockActivityRepository.Object);

            _mockUnitOfWork
                .Setup(m => m.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            //Act
            await _resetPasswordService.RequestByEmailAsync(email, clientIp, clientUserAgent, cancellationToken);

            //Assert
            _mockRateLimitService.Verify(m => m.CheckOnRateLimitAsync(clientIp, ActivityType.RequestResetPasswordByEmail, 5, 24, cancellationToken), Times.Once());

            _mockUserRepository.Verify(m => m.GetByEmail(email, cancellationToken), Times.Once());

            _mockUnitOfWork.Verify(m => m.ResetPasswords, Times.Never());

            _mockActivityRepository.Verify(m => m.AddAsync(
                It.Is<Activity>(
                    a => a.UserAgent == clientUserAgent &&
                    a.Ip == clientIp &&
                    a.Decription == $"Email Address: {email} not found in Database, since this requested received" &&
                    a.ResetPassword == null
                    ),
                It.IsAny<CancellationToken>()), Times.Once());

            _mockUnitOfWork.Verify(m => m.CompleteAsync(default), Times.Once());

            mockResetPasswordRepository.Verify(m => m.AddAsync(It.IsAny<ResetPassword>(), It.IsAny<CancellationToken>()), Times.Never());

            _mockBackgroundTaskQueue.Verify(m => m.QueueSendMail(It.IsAny<Func<CancellationToken, Task>>()), Times.Never());
        }

        /// <summary>
        /// Asserts that <see cref="ResetPasswordService.ChangePasswordAsync(ChangePasswordRequest, string, string, CancellationToken)"/>
        /// should return <see cref="Task<bool>"/> true
        ///  when <see cref="ResetPassword"/> is not null
        /// when change password successfull.
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task ChangePasswordAsync_ShouldReturnTrue_WhenChangePasswordSuccessfully()
        {
            //Arrange
            string clientIp = "TestClientIp";
            string clientUserAgent = "TestClientUserAgent";
            var cancellationToken = CancellationToken.None;

            var mockResetPasswordRepository = new Mock<IResetPasswordRepository>();

            var userId = Guid.NewGuid();
            var user = new User();

            var changePasswordRequest = new ChangePasswordRequest()
            {
                Key = Cryptography.EncodeBase64(userId.ToString()),
                Password = "TestPassword",
                Secret = "TestSecret"
            };

            var resetPasswordId = Guid.NewGuid();
            var resetPasssword = new ResetPassword()
            {
                ResetPasswordId = resetPasswordId,
                Secret = Cryptography.GenerateSecret(32),
                Expiry = DateTime.UtcNow.AddHours(1),
                User = user,
            };

            mockResetPasswordRepository
                .Setup(rp => rp.FindByUserIdAndSecretAsync(It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(resetPasssword);

            _mockUnitOfWork
                .Setup(rp => rp.ResetPasswords)
                .Returns(mockResetPasswordRepository.Object);

            var activityId = Guid.NewGuid();
            var activity = new Activity() { ActivityId = activityId };

            _mockActivityRepository
               .Setup(a => a.AddAsync(It.IsAny<Activity>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(activity);

            mockResetPasswordRepository
                .Setup(rp => rp.Update(It.IsAny<ResetPassword>()))
                .Returns(resetPasssword);

            _mockUserRepository
                .Setup(u => u.Update(It.IsAny<User>()))
                .Returns(user);

            _mockUnitOfWork
             .Setup(m => m.CompleteAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync(1);

            //Act
            var changePasswordResult = await _resetPasswordService.ChangePasswordAsync
                                                                        (changePasswordRequest,
                                                                        clientIp,
                                                                        clientUserAgent,
                                                                        cancellationToken);

            //Assert
            Assert.True(changePasswordResult);

            mockResetPasswordRepository.Verify(rp => rp.FindByUserIdAndSecretAsync(userId, changePasswordRequest.Secret, cancellationToken), Times.Once());
            mockResetPasswordRepository.Verify(rp => rp.Update(resetPasssword), Times.Once());
            _mockUnitOfWork.Verify(rp => rp.ResetPasswords, Times.Exactly(2));

            _mockActivityRepository.Verify(a => a.AddAsync(
                It.Is<Activity>(a => a.UserAgent == clientUserAgent &&
                a.Ip == clientIp &&
                a.User == user &&
                a.Type == ActivityType.ClientChangedPassword &&
                a.Decription == $"Client Accept the Reset Password link and successfully changed password." &&
                a.ResetPassword != null),
                It.IsAny<CancellationToken>()), Times.Once());

            _mockUserRepository.Verify(u => u.Update(user), Times.Once());

            _mockUnitOfWork.Verify(u => u.CompleteAsync(cancellationToken), Times.Once());

        }

        /// <summary>
        /// Asserts that <see cref="ResetPasswordService.ChangePasswordAsync(ChangePasswordRequest, string, string, CancellationToken)"/>
        /// should return <see cref="Task<bool>"/> false
        /// when <see cref="ResetPassword"/> is null
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task ChangePasswordAsync_ShouldReturnfalse_WhenResetPasswordIsNull()
        {

            //Arrange
            string clientIp = "TestClientIp";
            string clientUserAgent = "TestClientUserAgent";
            var cancellationToken = CancellationToken.None;

            var mockResetPasswordRepository = new Mock<IResetPasswordRepository>();
            var mockActivityRepository = new Mock<IActivityRepository>();

            var userId = Guid.NewGuid();
            var user = new User();

            var changePasswordRequest = new ChangePasswordRequest()
            {
                Key = Cryptography.EncodeBase64(userId.ToString()),
                Password = "TestPassword",
                Secret = "TestSecret"
            };

            mockResetPasswordRepository
                .Setup(rp => rp.FindByUserIdAndSecretAsync(It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as ResetPassword);

            _mockUnitOfWork
               .Setup(rp => rp.ResetPasswords)
               .Returns(mockResetPasswordRepository.Object);

            //Act
            var changePasswordResult = await _resetPasswordService.ChangePasswordAsync(changePasswordRequest, clientIp, clientUserAgent, cancellationToken);

            //Assert
            Assert.False(changePasswordResult);
            mockResetPasswordRepository.Verify(rp => rp.FindByUserIdAndSecretAsync(userId, changePasswordRequest.Secret, cancellationToken), Times.Once());
            mockResetPasswordRepository.Verify(rp => rp.Update(It.IsAny<ResetPassword>()), Times.Never());

            _mockUnitOfWork.Verify(rp => rp.ResetPasswords, Times.Once());

            mockActivityRepository.Verify(a => a.AddAsync(
                It.IsAny<Activity>(),
                It.IsAny<CancellationToken>()), Times.Never());

            _mockUnitOfWork.Verify(rp => rp.Activities, Times.Never());

            _mockUserRepository.Verify(u => u.Update(user), Times.Never());

            _mockUnitOfWork.Verify(u => u.CompleteAsync(cancellationToken), Times.Never());
        }

        /// <summary>
        /// Asserts that <see cref="ResetPasswordService.DeclineAsync(DeclineRequest, string, string, CancellationToken)"/>
        /// return nothing
        /// updates resetpassword status to false when user declines the request
        /// and add the Activity with description "Client Reject the Reset Password link and ResetPassword changed to Inactive".
        /// when <see cref="ResetPassword"/> is not null
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task DeclineAsync_ReturnNothingUpdateResetPasswordAndAddActivity_WhenResetPasswordIsNotNull()
        {
            //Arrange
            string clientIp = "TestClientIp";
            string clientUserAgent = "TestClientUserAgent";
            var cancellationToken = CancellationToken.None;

            var mockResetPasswordRepository = new Mock<IResetPasswordRepository>();
            var mockUserRepository = new Mock<IUserRepository>();

            var userId = Guid.NewGuid();
            var user = new User();

            var declineRequest = new DeclineRequest()
            {
                Key = Cryptography.EncodeBase64(userId.ToString()),
                Secret = "TestSecret"
            };

            var resetPasswordId = Guid.NewGuid();
            var resetPasssword = new ResetPassword()
            {
                ResetPasswordId = resetPasswordId,
                Secret = Cryptography.GenerateSecret(32),
                Expiry = DateTime.UtcNow.AddHours(1),
                User = user,
            };

            mockResetPasswordRepository
                .Setup(rp => rp.FindByUserIdAndSecretAsync(It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(resetPasssword);

            _mockUnitOfWork
                .Setup(rp => rp.ResetPasswords)
                .Returns(mockResetPasswordRepository.Object);

            var activityId = Guid.NewGuid();
            var activity = new Activity() { ActivityId = activityId };

            _mockActivityRepository
               .Setup(a => a.AddAsync(It.IsAny<Activity>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(activity);

            mockResetPasswordRepository
                .Setup(rp => rp.Update(It.IsAny<ResetPassword>()))
                .Returns(resetPasssword);

            _mockUnitOfWork
             .Setup(m => m.CompleteAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync(1);

            //Act
            await _resetPasswordService.DeclineAsync(declineRequest, clientIp, clientUserAgent, cancellationToken);

            //Assert
            mockResetPasswordRepository.Verify(rp => rp.FindByUserIdAndSecretAsync(userId, declineRequest.Secret, cancellationToken), Times.Once());
            mockResetPasswordRepository.Verify(rp => rp.Update(resetPasssword), Times.Once());

            _mockUnitOfWork.Verify(rp => rp.ResetPasswords, Times.Exactly(2));

            _mockActivityRepository.Verify(a => a.AddAsync(
                It.Is<Activity>(a => a.UserAgent == clientUserAgent &&
                a.Ip == clientIp &&
                a.User == user &&
                a.Type == ActivityType.ClientRejectRequestResetPassword &&
                a.Decription == $"Client Reject the Reset Password link and ResetPassword changed to Inactive." &&
                a.ResetPassword != null),
                It.IsAny<CancellationToken>()), Times.Once());

            _mockUnitOfWork.Verify(u => u.CompleteAsync(cancellationToken), Times.Once());

            _mockUserRepository.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Asserts that <see cref="ResetPasswordService.DeclineAsync(DeclineRequest, string, string, CancellationToken)"/>
        /// return nothing and do nothing
        /// when <see cref="ResetPassword"/> is null
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task DeclineAsync_ReturnNothingAndUpdateNothing_WhenResetPasswordIsNull()
        {
            //Arrange
            string clientIp = "TestClientIp";
            string clientUserAgent = "TestClientUserAgent";
            var cancellationToken = CancellationToken.None;

            var mockResetPasswordRepository = new Mock<IResetPasswordRepository>();
            var mockActivityRepository = new Mock<IActivityRepository>();

            var userId = Guid.NewGuid();
            var user = new User();

            var declineRequest = new DeclineRequest()
            {
                Key = Cryptography.EncodeBase64(userId.ToString()),
                Secret = "TestSecret"
            };

            mockResetPasswordRepository
                .Setup(rp => rp.FindByUserIdAndSecretAsync(It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as ResetPassword);

            _mockUnitOfWork
               .Setup(rp => rp.ResetPasswords)
               .Returns(mockResetPasswordRepository.Object);

            //Act
            await _resetPasswordService.DeclineAsync(declineRequest, clientIp, clientUserAgent, cancellationToken);

            //Assert

            mockResetPasswordRepository.Verify(rp => rp.FindByUserIdAndSecretAsync(userId, declineRequest.Secret, cancellationToken), Times.Once());
            mockResetPasswordRepository.Verify(rp => rp.Update(It.IsAny<ResetPassword>()), Times.Never());

            _mockUnitOfWork.Verify(rp => rp.ResetPasswords, Times.Once());

            mockActivityRepository.Verify(a => a.AddAsync(
                It.IsAny<Activity>(),
                It.IsAny<CancellationToken>()), Times.Never());

            _mockUnitOfWork.Verify(rp => rp.Activities, Times.Never());

            _mockUnitOfWork.Verify(u => u.CompleteAsync(cancellationToken), Times.Never());

            _mockUserRepository.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Asserts that <see cref="ResetPasswordService.NewPasswordAsync(NewPasswordRequest, string, string, CancellationToken)"/>
        /// should return <see cref="Task<bool>"/> true
        ///  when old password is correct
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task NewPasswordAsync_ShouldReturnTrue_WhenNewPasswordSet()
        {
            //Arrange
            string clientIp = "TestClientIp";
            string clientUserAgent = "TestClientUserAgent";
            var cancellationToken = CancellationToken.None;

            var request = new NewPasswordRequest()
            {
                OldPassword = "OldPassword",
                NewPassword = "NewPassword"
            };

            var userId = Guid.NewGuid();
            var salt = "SaltTest";
            var user = new User
            {
                UserId = userId,
                PasswordSalt = salt,
                PasswordHash = Cryptography.CreateHash(request.OldPassword, salt)
            };
            var userSession = new UserSession
            {
                User = user
            };

            _mockCurrentUserSessionService.Setup(rp => rp.UserSession).Returns(userSession);

            var activityId = Guid.NewGuid();
            var activity = new Activity() { ActivityId = activityId, Type = ActivityType.ClientSetNewPassword };

            _mockActivityRepository
               .Setup(a => a.AddAsync(It.IsAny<Activity>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(activity);
            _mockUserRepository
                .Setup(u => u.Update(It.IsAny<User>()))
                .Returns(user);
            _mockUnitOfWork
             .Setup(m => m.CompleteAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync(1);

            //Act
            var newPasswordResult = await _resetPasswordService
                .NewPasswordAsync(request, clientIp, clientUserAgent, cancellationToken);

            //Assert
            Assert.True(newPasswordResult);
            _mockCurrentUserSessionService.Verify(rp => rp.UserSession, Times.Once);
            _mockActivityRepository.Verify(a => a.AddAsync(
                It.Is<Activity>(a => a.UserAgent == clientUserAgent &&
                a.Ip == clientIp &&
                a.UserId == user.UserId &&
                a.Type == ActivityType.ClientSetNewPassword &&
                a.Decription == $"Client successfully set new password." &&
                a.ResetPassword == null),
                It.IsAny<CancellationToken>()), Times.Once());
            _mockUserRepository.Verify(u => u.Update(user), Times.Once());
            _mockUnitOfWork.Verify(u => u.CompleteAsync(cancellationToken), Times.Once());
        }

        /// <summary>
        /// Asserts that <see cref="ResetPasswordService.NewPasswordAsync(NewPasswordRequest, string, string, CancellationToken)"/>
        /// should return <see cref="Task<bool>"/> false
        ///  when old password is correct
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task NewPasswordAsync_ShouldReturnFalse_WhenOldPasswordIsWrong()
        {
            //Arrange
            string clientIp = "TestClientIp";
            string clientUserAgent = "TestClientUserAgent";
            var cancellationToken = CancellationToken.None;

            var request = new NewPasswordRequest()
            {
                OldPassword = "OldPassword",
                NewPassword = "NewPassword"
            };

            var userId = Guid.NewGuid();
            var salt = "SaltTest";
            var user = new User
            {
                UserId = userId,
                PasswordSalt = salt,
                PasswordHash = Cryptography.CreateHash("SomethingElse", salt)
            };
            var userSession = new UserSession
            {
                User = user
            };

            _mockCurrentUserSessionService.Setup(rp => rp.UserSession).Returns(userSession);

            var activityId = Guid.NewGuid();
            var activity = new Activity() { ActivityId = activityId, Type = ActivityType.ClientSetNewPassword };

            _mockActivityRepository
               .Setup(a => a.AddAsync(It.IsAny<Activity>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(activity);
            _mockUnitOfWork
             .Setup(m => m.CompleteAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync(1);

            //Act
            var newPasswordResult = await _resetPasswordService
                .NewPasswordAsync(request, clientIp, clientUserAgent, cancellationToken);

            //Assert
            Assert.False(newPasswordResult);
            _mockCurrentUserSessionService.Verify(rp => rp.UserSession, Times.Once);
            _mockActivityRepository.Verify(a => a.AddAsync(
                It.Is<Activity>(a => a.UserAgent == clientUserAgent &&
                a.Ip == clientIp &&
                a.UserId == user.UserId &&
                a.Type == ActivityType.ClientSetNewPassword &&
                a.Decription == $"Client entered wrong old password." &&
                a.ResetPassword == null),
                It.IsAny<CancellationToken>()), Times.Once());
            _mockUnitOfWork.Verify(u => u.CompleteAsync(cancellationToken), Times.Once());
        }
    }
}
