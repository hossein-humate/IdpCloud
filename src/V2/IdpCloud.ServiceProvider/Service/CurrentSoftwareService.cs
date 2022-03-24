using IdpCloud.Common;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.DataProvider.Entity.SSO;
using IdpCloud.Sdk;
using IdpCloud.ServiceProvider.EntityService.Identity;
using IdpCloud.ServiceProvider.EntityService.SSO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace IdpCloud.ServiceProvider.Service
{
    public class CurrentSoftwareService : ICurrentSoftwareService
    {
        private Software _software;
        private readonly ISoftwareRepository _softwareRepository;
        private readonly IJwtSettingRepository _jwtSettingRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentSoftwareService(ISoftwareRepository softwareRepository,
            IJwtSettingRepository jwtSettingRepository,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _softwareRepository = softwareRepository;
            _jwtSettingRepository = jwtSettingRepository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public Software Software
        {
            get
            {
                if (_software == null)
                {
                    var headers = _httpContextAccessor?.HttpContext?.Request?.Headers;
                    var clientId = headers != null && headers.ContainsKey(ApiManager.HeaderClientId)
                                        ? headers[ApiManager.HeaderClientId].ToString()
                                        : null;

                    if (!string.IsNullOrEmpty(clientId))
                    {
                        var software = _softwareRepository.GetSoftwareById(clientId, default).GetAwaiter().GetResult();

                        if (software != null && software.JwtSetting == null)
                        {
                            var softwareId = _configuration["Application:SoftwareId"].ToGuid();
                            software.JwtSetting = _jwtSettingRepository.GetBySoftwareId(softwareId).GetAwaiter().GetResult();
                        }

                        _software = software;
                    }
                }

                return _software;
            }
        }
    }
}
