using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.BackgroundWorker
{
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);
        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
        void QueueSendMail(Func<CancellationToken, Task> workItem);
        Task<Func<CancellationToken, Task>> DequeueSendMailAsync(CancellationToken cancellationToken);
    }
}