using IdpCloud.DataProvider.Entity.Enum;
using IdpCloud.DataProvider.Entity.Security;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.Service.Security
{
    /// <inheritdoc/>
    public class RateLimitService : IRateLimitService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RateLimitService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CheckOnRateLimitAsync(
            string ip,
            ActivityType type,
            int limitCount = 5,
            int releaseByhours = 24,
            CancellationToken cancellationToken = default)
        {
            var count = await _unitOfWork.Activities.CountLastHourOfIpAsync(ip, type, cancellationToken);
            if (count >= limitCount)
            {
                await _unitOfWork.BanLists.AddAsync(new BanList
                {
                    CreateDate = DateTime.UtcNow,
                    Ip = ip,
                    ReleaseDate = DateTime.UtcNow.AddHours(releaseByhours)
                });
                await _unitOfWork.CompleteAsync(cancellationToken);
                return true;
            }
            return false;
        }
    }
}
