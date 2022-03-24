using IdpCloud.Common.Settings;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.ServiceProvider.EntityService.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.Service.Identity
{
    ///<inheritdoc/>
    public class UserSoftwareService : IUserSoftwareService
    {
        private readonly GlobalParameterSetting _globalParameter;
        private readonly IUserSoftwareRepository _userSoftwareRepository;
        public UserSoftwareService(
            IUserSoftwareRepository userSoftwareRepository,
            IOptions<GlobalParameterSetting> globalParameter)
        {
            _userSoftwareRepository = userSoftwareRepository ?? throw new ArgumentNullException(nameof(userSoftwareRepository));
            _globalParameter = globalParameter == null
                ? throw new ArgumentNullException(nameof(globalParameter)) : globalParameter.Value;
        }

        public async Task<IEnumerable<Software>> SoftwareList(Guid userId, CancellationToken cancellationToken = default)
        {
            var softwares = await _userSoftwareRepository.GetSoftwareListByUserId(userId, cancellationToken);
            return softwares.Where(s => s.SoftwareId != _globalParameter.SsoSoftwareId);
        }
    }
}
