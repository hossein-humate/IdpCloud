using AutoMapper;
using IdpCloud.Common;
using IdpCloud.Common.Settings;
using IdpCloud.DataProvider.Entity.Enum;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.DataProvider.Entity.Security;
using IdpCloud.Sdk.Model;
using IdpCloud.Sdk.Model.Identity.Request.User;
using IdpCloud.Sdk.Model.SSO.Request.User;
using IdpCloud.ServiceProvider.BackgroundWorker;
using IdpCloud.ServiceProvider.EntityService.Identity;
using IdpCloud.ServiceProvider.EntityService.Security;
using IdpCloud.ServiceProvider.InternalService.Mail;
using IdpCloud.ServiceProvider.Service.Security;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.Service.Identity
{
    ///<inheritdoc/>
    public class UserService : RandomPasswordGenerator, IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly IBackgroundTaskQueue _backgroundTask;
        private readonly IEmailService _mailService;
        private readonly IRateLimitService _rateLimitService;
        private readonly GlobalParameterSetting _globalParameterSetting;
        private readonly MailServiceSetting _mailServiceSetting;
        private readonly IMapper _mapper;
        private readonly IRandomPasswordGenerator _randomPasswordGenerator;
        private readonly IResetPasswordRepository _resetPasswordRepository;
        public UserService(
            IUnitOfWork unitOfWork,
            IBackgroundTaskQueue backgroundTask,
            IEmailService mailService,
            IRateLimitService rateLimitService,
            IUserRepository userRepository,
            IRoleRepository userRoleRepository,
            IActivityRepository activityRepository,
            IOptions<GlobalParameterSetting> globalParameterSettingOptions,
            IOptions<MailServiceSetting> mailServiceSettingOptions,
            IMapper mapper,
            IRandomPasswordGenerator randomPasswordGenerator,
            IResetPasswordRepository resetPasswordRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _backgroundTask = backgroundTask ?? throw new ArgumentNullException(nameof(backgroundTask));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _rateLimitService = rateLimitService ?? throw new ArgumentNullException(nameof(rateLimitService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _roleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
            _activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            _globalParameterSetting = globalParameterSettingOptions == null
                                        ? throw new ArgumentNullException(nameof(globalParameterSettingOptions))
                                        : globalParameterSettingOptions.Value;
            _mailServiceSetting = mailServiceSettingOptions == null
                                       ? throw new ArgumentNullException(nameof(mailServiceSettingOptions))
                                       : mailServiceSettingOptions.Value;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _randomPasswordGenerator = randomPasswordGenerator ?? throw new ArgumentNullException(nameof(randomPasswordGenerator));
            _resetPasswordRepository = resetPasswordRepository ?? throw new ArgumentNullException(nameof(resetPasswordRepository));
        }

        ///<inheritdoc/>
        public async Task<RequestResult> ConfirmEmail(string key, string secret, CancellationToken cancellationToken = default)
        {
            var result = RequestResult.RequestSuccessful;

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(secret))
            {
                result = RequestResult.EmptyEntryNotAllowed;
            }
            else
            {
                var userId = Cryptography.DecodeBase64(key).ToGuid();

                var user = await _userRepository.GetByEmailConfirmationSecretAndId(userId, secret, cancellationToken);

                result = ValidateUser(result, user);

                if (result == RequestResult.RequestSuccessful)
                {
                    await UpdateConfirmEmail(user, cancellationToken);
                }
            }
            return result;
        }

        ///<inheritdoc/>
        public async Task<RequestResult> Register(RegisterRequest request, string ip, string userAgent, CancellationToken cancellationToken = default)
        {
            var result = RequestResult.RequestSuccessful;

            if (!await _rateLimitService.CheckOnRateLimitAsync(ip, ActivityType.EmailConfirmationOnRegister,
                cancellationToken: cancellationToken))
            {
                if (await _userRepository.GetByUsername(request.Username, cancellationToken) != null)
                {
                    result = RequestResult.UsernameExist;
                }
                else
                {
                    var sameEmailuser = await _userRepository.GetByEmail(request.Email, cancellationToken);
                    if (sameEmailuser != null
                       && sameEmailuser.EmailConfirmed == false
                       && sameEmailuser.EmailConfirmationExpiry < DateTime.UtcNow
                       && sameEmailuser.LoginTimes == 0)
                    {
                        _userRepository.Delete(sameEmailuser);
                        sameEmailuser = null;
                    }

                    if (sameEmailuser == null)
                    {
                        var organisation = new Organisation
                        {
                            Name = request.OrganisationName,
                            BillingEmail = request.OrganisationBillingEmail,
                            Phone = request.OrganisationPhone,
                            BillingAddress = request.OrganisationBillingAddress,
                            VatNumber = request.OrganisationVatNumber
                        };

                        var salt = Cryptography.GenerateSecret();
                        var user = new User
                        {
                            Organisation = organisation,
                            PasswordHash = request.Password.CreateHash(salt),
                            PasswordSalt = salt,
                            Firstname = request.Firstname,
                            Lastname = request.Lastname,
                            Email = request.Email,
                            Username = request.Username.Trim().ToLower(),
                            UpdateDate = DateTime.UtcNow,
                            RegisterIp = ip,
                            RegisterDate = DateTime.UtcNow,
                            Status = UserStatus.Active,
                            EmailConfirmationExpiry = DateTime.UtcNow.AddDays(3),
                            EmailConfirmationSecret = Cryptography.GenerateSecret(32)
                        };
                        await _userRepository.Add(user);

                        var userRole = new UserRole
                        {
                            User = user,
                            RoleId = _globalParameterSetting.SsoPublicRoleId
                        };
                        await _roleRepository.AddUserRole(userRole);

                        var activity = new Activity
                        {
                            Ip = ip,
                            UserAgent = userAgent,
                            Type = ActivityType.EmailConfirmationOnRegister,
                            UserId = user.UserId,
                            User = user,
                            Decription = $"Email confirmation link has been sent to {user.Email}"
                        };

                        await _activityRepository.AddAsync(activity, cancellationToken);

                        await _unitOfWork.CompleteAsync(cancellationToken);

                        SendLinkByEmail(activity);
                    }
                    else
                    {
                        result = RequestResult.EmailExist;
                    }
                }
            }

            return result;
        }

        ///<inheritdoc/>
        public async Task<RequestResult> SendEmailConfirmation(SendEmailConfirmationRequest request, string ip, string userAgent, CancellationToken cancellationToken = default)
        {
            var result = RequestResult.RequestSuccessful;

            if (!await _rateLimitService.CheckOnRateLimitAsync(ip, ActivityType.ResendEmailConfirmationLink,
                cancellationToken: cancellationToken))
            {
                var user = await _userRepository.GetUserByUserNameOrEmailAndPassword(
                    request.UsernameOrEmail, request.Password, cancellationToken);
                if (user != null)
                {
                    if (!user.EmailConfirmed)
                    {
                        user.EmailConfirmationExpiry = DateTime.UtcNow.AddDays(3);
                        user.EmailConfirmationSecret = Cryptography.GenerateSecret(32);
                        _userRepository.Update(user);

                        var activity = new Activity
                        {
                            Ip = ip,
                            UserAgent = userAgent,
                            Type = ActivityType.ResendEmailConfirmationLink,
                            UserId = user.UserId,
                            User = user,
                            Decription = $"Resend Email confirmation link to {user.Email}"
                        };
                        await _activityRepository.AddAsync(activity, cancellationToken);
                        await _unitOfWork.CompleteAsync(cancellationToken);
                        SendLinkByEmail(activity);
                    }
                    else
                    {
                        result = RequestResult.EmailAddressConfirmed;
                    }
                }
                else
                {
                    result = RequestResult.EmailUsernameOrPasswordWrong;
                }
            }

            return result;
        }

        ///<inheritdoc/>
        public async Task<User> Create(CreateUserRequest createUserRequest, CancellationToken cancellationToken = default)
        {
            var user = _mapper.Map<User>(createUserRequest);

            var salt = Cryptography.GenerateSecret();
            user.PasswordHash = _randomPasswordGenerator.RandomPassword().CreateHash(salt);
            user.PasswordSalt = salt;
            user.Status = UserStatus.Active;
            var addedUser = await _userRepository.Add(user);

            var userRole = new UserRole
            {
                User = user,
                RoleId = new Guid(createUserRequest.RoleId)
            };

            await _roleRepository.AddUserRole(userRole);

            var resetPassword = await _resetPasswordRepository.AddAsync(new ResetPassword
            {
                Expiry = DateTime.UtcNow.AddHours(1),
                User = user,
                Secret = Cryptography.GenerateSecret(32)
            }, cancellationToken);

            await _unitOfWork.CompleteAsync(cancellationToken);

            SendResetPasswordEmail(user, resetPassword);

            return addedUser;
        }

        /// <inheritdoc />
        public async Task<User> Update(UpdateUserRequest request, CancellationToken cancellationToken = default)
        {
            var user = _mapper.Map<User>(request);

            var updateUser = await _userRepository.FindById(request.UserId, cancellationToken);

            updateUser.Username = request.Username;
            updateUser.Firstname = request.Firstname;
            updateUser.Lastname = request.Lastname;
            updateUser.Email = request.Email;
            updateUser.Mobile = request.Mobile;

            _userRepository.Update(updateUser);
            await UpdateUserRole(request, updateUser, cancellationToken);

            await _unitOfWork.CompleteAsync(cancellationToken);
            return user;
        }

        private async Task UpdateUserRole(UpdateUserRequest request, User user, CancellationToken cancellationToken)
        {
            var userRoleExist = await _roleRepository.FindByUserId(request.UserId, cancellationToken);

            if (userRoleExist != null)
            {
                if (userRoleExist.RoleId != request.RoleId)
                {
                    userRoleExist.DeleteDate = DateTime.UtcNow;
                    _roleRepository.DeleteUserRole(userRoleExist);

                    var userRole = new UserRole
                    {
                        User = user,
                        RoleId = request.RoleId,
                    };

                    await _roleRepository.AddUserRole(userRole);
                }
            }
        }

        /// <inheritdoc />
        public Task<UserPaginationResult> GetAll(UserPaginationParam paginationParam, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        private static RequestResult ValidateUser(RequestResult result, User user)
        {
            if (user == null)
            {
                result = RequestResult.NotAllowedThisOperation;
            }
            else
            {
                if (user.EmailConfirmed)
                {
                    result = RequestResult.ConfirmationCompletedBefore;
                }
                else
                {
                    if (user.EmailConfirmationExpiry < DateTime.Now)
                    {
                        result = RequestResult.ConfirmationLinkExpired;
                    }
                }
            }

            return result;
        }

        private async Task UpdateConfirmEmail(User user, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = true;
            _userRepository.Update(user);
            await _unitOfWork.CompleteAsync(cancellationToken);

            _backgroundTask.QueueSendMail(async (token) =>
            {
                var parameters = new object[]
                {
                    user.Username,
                   _mailServiceSetting.FromName

                };
                await _mailService.SendEmail(_mailService.WelcomeUser,
                   toEmailAddress: new[] { user.Email },
                   subject: "Basemap Identity Provider Welcome",
                   attachments: default,
                   parameters: parameters
                   );
            });
        }

        /// <summary>
        /// Add new background task to the <see cref="IBackgroundTaskQueue"/> in category of 
        /// <see cref="SendMailHostedService"/> workitem for sending Email Confirmation Link 
        /// to provided register user activity information
        /// </summary>
        /// <param name="activity">Input arg for Register New User activity</param>
        private void SendLinkByEmail(Activity activity)
        {
            _backgroundTask.QueueSendMail(async (token) =>
            {
                string key = Cryptography.EncodeBase64(activity.User.UserId.ToString());
                string secret = WebUtility.UrlEncode(activity.User.EmailConfirmationSecret);
                var parameters = new object[]
                {
                    activity.User.Username,
                   _globalParameterSetting.SsoConfirmEmailUrl + $"?key={key}&secret={secret}"
                };

                await _mailService.SendEmail(_mailService.EmailConfirmation,
                    new[] { activity.User.Email },
                    "Basemap Identity Provider Email Address Confirmation",
                    attachments: default,
                    parameters);
            });
        }

        private void SendResetPasswordEmail(User user, ResetPassword resetPassword)
        {
            _backgroundTask.QueueSendMail(async (token) =>
            {
                string key = Cryptography.EncodeBase64(user.UserId.ToString());
                string secret = WebUtility.UrlEncode(resetPassword.Secret);
                var parameters = new object[]
                {
                    user.Firstname,
                    $"{_globalParameterSetting.SsoChangePasswordUrl}?key={key}&secret={secret}",

                };
                await _mailService.SendEmail(_mailService.NewUserRegister,
                   toEmailAddress: new[] { user.Email },
                   subject: "Basemap Identity Provider Reset Password",
                   attachments: default,
                   parameters: parameters
                   );
            });
        }

    }
}
