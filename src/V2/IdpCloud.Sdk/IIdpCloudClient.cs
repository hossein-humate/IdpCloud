using IdpCloud.Sdk.Model;
using IdpCloud.Sdk.Model.Identity.Request.User;
using IdpCloud.Sdk.Model.Identity.Response.User;
using IdpCloud.Sdk.Model.SSO.Request.UserSession;
using IdpCloud.Sdk.Model.SSO.Response;
using IdpCloud.Sdk.Model.SSO.Response.UserSession;
using System.Threading;
using System.Threading.Tasks;

namespace Basemap.Identity.Sdk
{
    public interface IIdpCloudClient
    {
        Task<RegisterAndLoginResponse> RegisterAndLoginAsync(RegisterAndLoginRequest request,
            CancellationToken cancellationToken = default);

        Task<LoginResponse> LoginAsync(LoginRequest request,
            CancellationToken cancellationToken = default);

        Task<BaseResponse> AvailableUserNameAsync(string userName,
            CancellationToken cancellationToken = default);

        Task<GetBySoftwareIdResponse> UserGetBySoftwareIdAsync(
            string token, string softwareId, CancellationToken cancellationToken = default);

        Task<BaseResponse> UserDeleteAsync(
            string token, string userId, CancellationToken cancellationToken = default);

        Task<CreateResponse> UserCreateAsync(string token, CreateRequest request,
            CancellationToken cancellationToken = default);

        Task<BaseResponse> UserUpdateAsync(string token, UpdateRequest
            request, CancellationToken cancellationToken = default);

        Task<GetByIdResponse> UserGetByIdAsync(string token, string userId,
            CancellationToken cancellationToken = default);

        Task<GetAllResponse> UserGetAllAsync(string token,
            CancellationToken cancellationToken = default);

        Task<BaseResponse> UserUpdateProfileAsync(string token,
           UpdateProfileRequest request, CancellationToken cancellationToken = default);

        Task<BaseResponse> IsValidTokenAsync(string token,
        CancellationToken cancellationToken = default);

        Task<AuthUserResponse> AuthUserAsync(string token,
            CancellationToken cancellationToken = default);

        Task<RefreshTokenResponse> UserSessionRefreshTokenAsync(string token, RefreshTokenRequest request, CancellationToken cancellationToken = default);

        Task<BaseResponse> ResetPasswordRequestByEmailAsync(string email,
            CancellationToken cancellationToken = default);
    }
}