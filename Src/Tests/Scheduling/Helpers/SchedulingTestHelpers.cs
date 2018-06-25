using System;
using Coravel.Scheduling.Schedule;

namespace Tests.Scheduling.Helpers
{
    public static class SchedulingTestHelpers
    {
        public static async Task RunScheduledTasksFromMinutes(Scheduler scheduler, int minutes){
            scheduler.RunAt(DateTime.Today.Add(TimeSpan.FromMinutes(minutes)));
        }

        public static async Task RunScheduledTasksFromDayHourMinutes(Scheduler scheduler, int days, int hours, int minutes){
            var daysSpan = TimeSpan.FromDays(days);
            var hoursSpan = TimeSpan.FromHours(hours);
            var minutesSpan = TimeSpan.FromMinutes(minutes);

            var combinedTimeSpan = daysSpan.Add(hoursSpan).Add(minutesSpan);

            scheduler.RunAt(DateTime.Today.Add(combinedTimeSpan));
        }
    }
}