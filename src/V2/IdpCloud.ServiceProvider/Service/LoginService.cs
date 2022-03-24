using AutoMapper;
using IdpCloud.Common;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.DataProvider.Entity.SSO;
using IdpCloud.Sdk.Model;
using IdpCloud.Sdk.Model.Identity.Request.User;
using IdpCloud.Sdk.Model.Identity.Response.User;
using IdpCloud.ServiceProvider.EntityService.BaseInfo;
using IdpCloud.ServiceProvider.EntityService.Identity;
using IdpCloud.ServiceProvider.EntityService.SSO;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.Service
{
    /// <inheritdoc />
    public class LoginService : ILoginService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentSoftwareService _currentSoftwareService;
        private readonly IJwtAuthenticationService _jwtAuthenticationService;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly IUserRepository _userRepository;
        public LoginService(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ICurrentSoftwareService currentSoftwareService,
            IJwtAuthenticationService jwtAuthenticationService,
            IUserSessionRepository userSessionRepository,
            ILanguageRepository languageRepository,
            IUserRepository userRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _currentSoftwareService = currentSoftwareService ?? throw new ArgumentNullException(nameof(currentSoftwareService));
            _jwtAuthenticationService = jwtAuthenticationService ?? throw new ArgumentNullException(nameof(jwtAuthenticationService));
            _userSessionRepository = userSessionRepository;
            _languageRepository = languageRepository;
            _userRepository = userRepository;
        }

        /// <inheritdoc />
        public async Task<LoginResponse> Login(LoginRequest request, string clientIp, string clientUserAgent, CancellationToken cancellationToken = default)
        {
            LoginResponse result = null;

            var user = await ValidateUser(request, clientIp, cancellationToken);
            if (user != null)
            {
                if (user.EmailConfirmed)
                {
                    var session = await CreateSession(user.UserId, clientIp, clientUserAgent, cancellationToken);

                    result = new LoginResponse
                    {
                        Token = await _jwtAuthenticationService.CreateTokenAuthenticationAsync(session, cancellationToken),
                        RefreshToken = session.RefreshToken,
                        User = _mapper.Map<Sdk.Model.Identity.User>(user),
                        Organisation = _mapper.Map<Sdk.Model.Identity.Organisation>(user.Organisation),
                        Role = _mapper.Map<Sdk.Model.Identity.Role>(user.UserRoles?.FirstOrDefault(ur =>
                            ur.Role.SoftwareId == session.SoftwareId)?.Role)
                    };

                    await _unitOfWork.CompleteAsync(cancellationToken);
                }
                else
                {
                    result = BaseResponseCollection.GetGenericeResponse<LoginResponse>(
                        RequestResult.EmailNotConfirmed);
                }
            }

            return result;
        }

        private async Task<User> ValidateUser(LoginRequest request, string clientIp, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetUserByUserNameOrEmailAndPassword(
                request.UsernameOrEmail,
                request.Password,
                cancellationToken);

            if (user != null)
            {
                user.LastLoginDate = DateTime.Now;
                user.LastLoginIp = clientIp;
                user.LoginTimes += 1;
                var language = await _languageRepository.FindById(request.LanguageId, cancellationToken);
                if (language != null)
                {
                    user.Language = language;
                }
                _userRepository.Update(user);
            }

            return user;
        }

        private async Task<UserSession> CreateSession(Guid userId, string clientIp, string clientUserAgent, CancellationToken cancellationToken = default)
        {
            var userSession = new UserSession
            {
                AuthType = AuthType.UserPass,
                Status = Status.Active,
                Ip = clientIp,
                UserAgent = clientUserAgent,
                SoftwareId = _currentSoftwareService.Software.SoftwareId,
                UserId = userId,
                RefreshToken = _currentSoftwareService.Software.JwtSetting.HasRefresh ? Cryptography.GenerateRefreshToken() : null
            };

            await _userSessionRepository.Add(userSession, cancellationToken);
            return userSession;
        }
    }
}

