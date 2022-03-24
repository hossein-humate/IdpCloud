using EntityServiceProvider;
using General;
using System;
using System.Threading;
using System.Threading.Tasks;
using Humate.RESTApi.Infrastructure.Authentication.Model;

namespace Humate.RESTApi.Infrastructure.Authentication
{
    public interface IApiAuthenticationService
    {
        Task<bool> ValidateSecretKeyAsync(string token = default,
            CancellationToken cancellationToken = default);
    }

    public class ApiAuthenticationService : IApiAuthenticationService
    {
        private readonly IAuthenticatedUser _authenticatedUser;
        private readonly IUnitOfWork _unitOfWork;
        public ApiAuthenticationService(IUnitOfWork unitOfWork, IAuthenticatedUser authenticatedUser)
        {
            _unitOfWork = unitOfWork;
            _authenticatedUser = authenticatedUser;
        }

        public async Task<bool> ValidateSecretKeyAsync(string token = default,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (token == default)
                    return false;
                var software = await _unitOfWork.Softwares.FindAsNoTrackingAsync(u =>
                     u.ApiKey == token, cancellationToken, s => s.Roles);
                if (software == null)
                {
                    return false;
                }

                if (software.KeyExpire.UnixTimeStampToDateTime() < DateTime.Now)
                {
                    return false;
                }

                _authenticatedUser.Software = software;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
