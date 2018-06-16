using System;
using System.Threading;

namespace Coravel.Scheduling.Timing
{
    internal class OneMinuteTimer : IDisposable
    {
        private static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(1);
        private object _lock = new object();
        private Timer _timer;
        private Action _callback;

        public OneMinuteTimer(Action callback)
        {
            this._callback = callback;
            this._timer = new Timer(this.ExecCallbackWithPausedTimer, null, TimeSpan.Zero, OneMinute);
        }

        private void ExecCallbackWithPausedTimer(object state) {
            // If callback is still running we'll just keep the timer going 
            // until the next iteration.
            if(Monitor.TryEnter(this._lock))
            {
                try {
                    this._callback();
                }
              finally {
                    Monitor.Exit(this._lock);
                }
            }
        }

        public void Stop(){
            this._timer?.Change(Timeout.Infinite, 0);
        }

        public void Dispose()
        {
            this._timer?.Dispose();
        }
    }
}