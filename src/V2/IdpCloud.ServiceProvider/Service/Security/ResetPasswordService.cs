using IdpCloud.Common;
using IdpCloud.Common.Settings;
using IdpCloud.DataProvider.Entity.Enum;
using IdpCloud.DataProvider.Entity.Security;
using IdpCloud.Sdk.Model.Security.Request.ResetPassword;
using IdpCloud.ServiceProvider.BackgroundWorker;
using IdpCloud.ServiceProvider.EntityService.Identity;
using IdpCloud.ServiceProvider.EntityService.Security;
using IdpCloud.ServiceProvider.InternalService.Mail;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.Service.Security
{
    /// <inheritdoc />
    public class ResetPasswordService : IResetPasswordService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBackgroundTaskQueue _backgroundTask;
        private readonly IEmailService _mailService;
        private readonly IRateLimitService _rateLimitService;
        private readonly IUserRepository _userRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly ICurrentUserSessionService _currentUserSessionService;
        private readonly GlobalParameterSetting _globalParameterSetting;

        public ResetPasswordService(
            IUnitOfWork unitOfWork,
            IBackgroundTaskQueue backgroundTask,
            IEmailService mailService,
            IRateLimitService rateLimitService,
            IUserRepository userRepository,
            IActivityRepository activityRepository,
            ICurrentUserSessionService currentUserSessionService,
            IOptions<GlobalParameterSetting> globalParameterSettingOptions)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _backgroundTask = backgroundTask ?? throw new ArgumentNullException(nameof(backgroundTask));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _rateLimitService = rateLimitService;
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            _currentUserSessionService = currentUserSessionService ?? throw new ArgumentNullException(nameof(currentUserSessionService));
            _globalParameterSetting = globalParameterSettingOptions == null
                                        ? throw new ArgumentNullException(nameof(globalParameterSettingOptions))
                                        : globalParameterSettingOptions.Value;
        }

        /// <inheritdoc/>
        public async Task RequestByEmailAsync(
            string email,
            string clientIp,
            string clientUserAgent,
            CancellationToken cancellationToken = default)
        {
            if (await _rateLimitService.CheckOnRateLimitAsync(clientIp, ActivityType.RequestResetPasswordByEmail,
                cancellationToken: cancellationToken))
            {
                return;
            }

            var activity = new Activity
            {
                UserAgent = clientUserAgent,
                Ip = clientIp,
                Type = ActivityType.RequestResetPasswordByEmail
            };

            var user = await _userRepository.GetByEmail(email, cancellationToken);
            if (user != null)
            {
                var resetPassword = await _unitOfWork.ResetPasswords.AddAsync(new ResetPassword
                {
                    Expiry = DateTime.UtcNow.AddHours(1),
                    User = user,
                    Secret = Cryptography.GenerateSecret(32)
                }, cancellationToken);
                activity.ResetPassword = resetPassword;
            }
            else
            {
                activity.Decription = $"Email Address: {email} not found in Database, since this requested received";
            }

            await _activityRepository.AddAsync(activity, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            if (activity.ResetPassword != null)
            {
                SendLinkByEmail(activity);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ChangePasswordAsync(
            ChangePasswordRequest request,
            string clientIp,
            string clientUserAgent,
            CancellationToken cancellationToken = default)
        {
            var resetPassword = await ValidateAndExtractRequest(request.Key, request.Secret, cancellationToken);
            if (resetPassword != null)
            {
                var activity = new Activity
                {
                    UserAgent = clientUserAgent,
                    Ip = clientIp,
                    Type = ActivityType.ClientChangedPassword,
                    User = resetPassword.User,
                    ResetPassword = resetPassword,
                    Decription = $"Client Accept the Reset Password link and successfully changed password."
                };
                await _activityRepository.AddAsync(activity, cancellationToken);

                resetPassword.Active = false;
                _unitOfWork.ResetPasswords.Update(resetPassword);

                resetPassword.User.PasswordSalt = Cryptography.GenerateSecret();
                resetPassword.User.PasswordHash = request.Password
                    .CreateHash(resetPassword.User.PasswordSalt);

                _userRepository.Update(resetPassword.User);

                await _unitOfWork.CompleteAsync(cancellationToken);
                return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public async Task DeclineAsync(DeclineRequest request, string clientIp, string clientUserAgent, CancellationToken cancellationToken = default)
        {
            var resetPassword = await ValidateAndExtractRequest(request.Key, request.Secret, cancellationToken);
            if (resetPassword != null)
            {
                var activity = new Activity
                {
                    UserAgent = clientUserAgent,
                    Ip = clientIp,
                    Type = ActivityType.ClientRejectRequestResetPassword,
                    User = resetPassword.User,
                    ResetPassword = resetPassword,
                    Decription = $"Client Reject the Reset Password link and ResetPassword changed to Inactive."
                };
                await _activityRepository.AddAsync(activity, cancellationToken);

                resetPassword.Active = false;
                _unitOfWork.ResetPasswords.Update(resetPassword);
                await _unitOfWork.CompleteAsync(cancellationToken);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> NewPasswordAsync(
            NewPasswordRequest request,
            string clientIp,
            string clientUserAgent,
            CancellationToken cancellationToken = default)
        {
            var user = _currentUserSessionService.UserSession.User;
            bool result = false;
            var activity = new Activity
            {
                UserAgent = clientUserAgent,
                Ip = clientIp,
                Type = ActivityType.ClientSetNewPassword,
                UserId = user.UserId
            };
            if (request.OldPassword.ValidateHash(user.PasswordSalt, user.PasswordHash))
            {
                user.UserSessions = default;
                user.PasswordSalt = Cryptography.GenerateSecret();
                user.PasswordHash = request.NewPassword.CreateHash(user.PasswordSalt);
                activity.Decription = "Client successfully set new password.";
                _userRepository.Update(user);
                result = true;
            }
            else
            {
                activity.Decription = "Client entered wrong old password.";
            }
            await _activityRepository.AddAsync(activity, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return result;
        }

        /// <summary>
        /// Validate the the requests parameters and the Resetpassword limitations on Expiry and Active status
        /// </summary>
        /// <param name="key">Represent the Request Key</param>
        /// <param name="secret">Represent the Request Secret</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns>Return a <see cref="Task"/> that is contain a <see cref="ResetPassword"/> record in the database that is valid and ready to use for change password or decline </returns>
        private async Task<ResetPassword> ValidateAndExtractRequest(string key, string secret, CancellationToken cancellationToken = default)
        {
            var userId = Cryptography.DecodeBase64(key.ToString()).ToGuid();
            var resetPassword = await _unitOfWork.ResetPasswords.FindByUserIdAndSecretAsync(userId, secret, cancellationToken);
            if (resetPassword != null && resetPassword.Expiry >= DateTime.UtcNow)
            {
                return resetPassword;
            }
            return null;
        }

        /// <summary>
        /// Add new background task to the <see cref="IBackgroundTaskQueue"/> in category of <see cref="SendMailHostedService"/> workitem for sending reset password request email to the account owner
        /// </summary>
        /// <param name="activity">Input arg for Reset password activity</param>
        private void SendLinkByEmail(Activity activity)
        {
            _backgroundTask.QueueSendMail(async (token) =>
            {
                string key = Cryptography.EncodeBase64(activity.ResetPassword.User.UserId.ToString());
                string secret = WebUtility.UrlEncode(activity.ResetPassword.Secret);
                var parameters = new object[]
                {
                    activity.ResetPassword.User.Username,
                    activity.Ip,
                    activity.CreateDate.ToString("D"),
                    activity.CreateDate.ToString("T"),
                    $"{_globalParameterSetting.SsoChangePasswordUrl}?key={key}&secret={secret}",
                    $"{_globalParameterSetting.SsoRejectResetPasswordUrl}?key={key}&secret={secret}"
                };
                await _mailService.SendEmail(_mailService.ResetPassword,
                   new[] { activity.ResetPassword.User.Email },
                    "Basemap Identity Provider Reset Password",
                    attachments: default,
                    parameters);
            });
        }
    }
}
