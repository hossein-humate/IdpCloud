using System;
using System.Timers;

namespace Humate.WASM.Shared.Component.Toast
{
    public class CountdownTimer : IDisposable
    {
        private Timer _timer;
        private readonly int _timeout;
        private int _percentComplete;

        public Action<int> OnTick;
        public Action OnElapsed;

        public CountdownTimer(int timeout)
        {
            _timeout = (timeout * 1000) / 100;
            _percentComplete = 0;
            SetupTimer();
        }

        public void Start()
        {
            _timer.Start();
        }

        private void SetupTimer()
        {
            _timer = new Timer(_timeout);
            _timer.Elapsed += HandleTick;
            _timer.AutoReset = false;
        }

        private void HandleTick(object sender, ElapsedEventArgs args)
        {
            _percentComplete++;
            OnTick?.Invoke(_percentComplete);

            if (_percentComplete == 100)
            {
                OnElapsed?.Invoke();
            }
            else
            {
                SetupTimer();
                Start();
            }
        }

        public void Dispose()
        {
            _timer.Dispose();
            _timer = null;
        }
    }
}