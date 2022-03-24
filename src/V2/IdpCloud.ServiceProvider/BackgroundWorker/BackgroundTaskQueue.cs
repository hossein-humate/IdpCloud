using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.BackgroundWorker
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly SemaphoreSlim _sendMailSignal = new SemaphoreSlim(0);

        private readonly ConcurrentQueue<Func<CancellationToken, Task>> _sendMailWorkItems =
            new ConcurrentQueue<Func<CancellationToken, Task>>();

        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        private readonly ConcurrentQueue<Func<CancellationToken, Task>> _workItems =
            new ConcurrentQueue<Func<CancellationToken, Task>>();

        public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
            _workItems.Enqueue(workItem);
            _signal.Release();
        }

        public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);
            return workItem;
        }

        public void QueueSendMail(Func<CancellationToken, Task> workItem)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
            _sendMailWorkItems.Enqueue(workItem);
            _sendMailSignal.Release();
        }

        public async Task<Func<CancellationToken, Task>> DequeueSendMailAsync(CancellationToken cancellationToken)
        {
            await _sendMailSignal.WaitAsync(cancellationToken);
            _sendMailWorkItems.TryDequeue(out var workItem);
            return workItem;
        }
    }
}