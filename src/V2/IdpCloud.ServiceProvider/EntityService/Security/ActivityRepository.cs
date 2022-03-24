using IdpCloud.DataProvider.DatabaseContext;
using IdpCloud.DataProvider.Entity.Enum;
using IdpCloud.DataProvider.Entity.Security;
using Microsoft.EntityFrameworkCore;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("IdpCloud.Tests")]
namespace IdpCloud.ServiceProvider.EntityService.Security
{
    /// <inheritdoc />
    internal class ActivityRepository : IActivityRepository
    {
        protected readonly DbSet<Activity> _activity;
        public ActivityRepository(EfCoreContext databaseContext)
        {
            _activity = databaseContext.Set<Activity>();
        }

        public async Task<Activity> AddAsync(Activity activity, CancellationToken cancellationToken = default)
        {
            return (await _activity.AddAsync(activity, cancellationToken)).Entity;
        }

        public async Task<int> CountLastHourOfIpAsync(
            string ip,
            ActivityType type,
            CancellationToken cancellationToken = default)
        {
            return await _activity.CountAsync(a =>
                ip.Equals(a.Ip) && a.CreateDate >= DateTime.UtcNow.AddHours(-1) && a.Type == type, cancellationToken);
        }
    }
}
