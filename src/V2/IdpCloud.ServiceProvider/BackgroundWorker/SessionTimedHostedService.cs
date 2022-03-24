using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.BackgroundWorker
{
    public class SessionTimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _services;
        private Timer _timer;

        public SessionTimedHostedService(IServiceProvider services, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SendMailHostedService>();
            _services = services;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Session Timed Hosted Service running.");

            _timer = new Timer(DoWorkAsync, null, TimeSpan.FromSeconds(60),
                TimeSpan.FromHours(2));

            await Task.CompletedTask;
        }

        private async void DoWorkAsync(object state)
        {
            try
            {
                //using var scope = _services.CreateScope();
                //var userSessionRepository = scope.ServiceProvider.GetRequiredService<IUserSessionRepository>();
                //var userSessions = await userSessionRepository.FindAllAsync(u => u.Status == Entity.SSO.Status.Active
                //      &&
                //      (
                //        u.CreateDate.AddMinutes(u.Software.JwtSetting.ExpireMinute) < DateTime.Now
                //            && !u.Software.JwtSetting.HasRefresh
                //        ||
                //        u.CreateDate.AddMinutes(u.Software.JwtSetting.ExpireMinute
                //            + u.Software.JwtSetting.RefreshExpireMinute) < DateTime.Now
                //            && u.Software.JwtSetting.HasRefresh
                //      ));
                //userSessions.ToList().ForEach(session => session.Status = Entity.SSO.Status.DeActive);
                //await userSessionRepository.SaveChangeAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError($"Session Timed Hosted Service raise error: {exception.Message} \n Stack Trace: {exception.StackTrace}");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Session Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}