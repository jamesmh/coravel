using System;
using System.Threading;
using System.Threading.Tasks;

namespace Coravel.Scheduling.Timing
{
    internal class OneMinuteTimer : IDisposable
    {
        private static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(1);
        private Timer _timer;
        private Func<Task> _callback;

        public OneMinuteTimer(Func<Task> callback)
        {
            this._callback = callback;
            this._timer = new Timer(this.ExecCallbackWithPausedTimer, null, TimeSpan.Zero, OneMinute);
        }

        private void ExecCallbackWithPausedTimer(object state) {
            this._callback().ConfigureAwait(false);
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