using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule.Interfaces;

namespace Coravel.Scheduling.Schedule
{
    public class ScheduledAsyncTask : IScheduleInterval, IScheduledTask
    {
        private ScheduledTask _scheduledTask;
        private Func<Task> _asyncTask;

        public ScheduledAsyncTask(Func<Task> task) {
            this._scheduledTask = ScheduledTask.WithEmptyTask();
        }

        public bool ShouldInvokeNow(DateTime utcNow) => this._scheduledTask.ShouldInvokeNow(utcNow);

        public async Task InvokeAsync() => await this._asyncTask();
        
        private void Dummy(){}

        public IScheduleRestriction Daily() => this._scheduledTask.Daily();

        public IScheduleRestriction DailyAt(int hour, int minute)=> this._scheduledTask.DailyAt(hour, minute);

        public IScheduleRestriction DailyAtHour(int hour) => this._scheduledTask.DailyAtHour(hour);

        public IScheduleRestriction EveryFifteenMinutes() => this._scheduledTask.EveryFifteenMinutes();

        public IScheduleRestriction EveryFiveMinutes() => this._scheduledTask.EveryFiveMinutes();

        public IScheduleRestriction EveryMinute() => this._scheduledTask.EveryMinute();

        public IScheduleRestriction EveryTenMinutes() => this._scheduledTask.EveryTenMinutes();

        public IScheduleRestriction EveryThirtyMinutes() => this._scheduledTask.EveryThirtyMinutes();

        public IScheduleRestriction Hourly() => this._scheduledTask.Hourly();

        public IScheduleRestriction HourlyAt(int minute) => this.HourlyAt(minute);

        public IScheduleRestriction Weekly() => this._scheduledTask.Weekly();
    }
}