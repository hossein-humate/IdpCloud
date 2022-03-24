using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.Service
{
    /// <inheritdoc />
    public class ApiAuthenticationService : IApiAuthenticationService
    {
        private readonly ICurrentSoftwareService _currentSoftwareService;
        public ApiAuthenticationService(ICurrentSoftwareService currentSoftwareService)
        {
            _currentSoftwareService = currentSoftwareService;
        }

        /// <inheritdoc />
        public bool ValidateSecretKey(string apiKey = default)
        {
            try
            {
                var currentSoftware = _currentSoftwareService.Software;
                if(currentSoftware == null)
                {
                    return false;
                };
                if (apiKey == default)
                    return false;
                if (currentSoftware.ApiKey != apiKey)
                    return false;
                return currentSoftware.KeyExpire >= DateTime.Now;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> HasClientIdAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                var software = _currentSoftwareService.Software;
                if (software == null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
