using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.BackgroundWorker
{
    public class SendMailHostedService : BackgroundService
    {
        private readonly ILogger _logger;

        public SendMailHostedService(IBackgroundTaskQueue taskQueue, ILoggerFactory loggerFactory)
        {
            TaskQueue = taskQueue;
            _logger = loggerFactory.CreateLogger<SendMailHostedService>();
        }

        public IBackgroundTaskQueue TaskQueue { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is starting.");
            while (!stoppingToken.IsCancellationRequested)
            {
                stoppingToken.Register(() => _logger.LogInformation("Queued Hosted Service is stopping."));
                var workItem = await TaskQueue.DequeueSendMailAsync(stoppingToken);
                try
                {
                    await workItem(stoppingToken);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Error occurred executing {nameof(workItem)}.");
                }
            }

            _logger.LogInformation("Queued Hosted Service is stopping.");
        }
    }
}