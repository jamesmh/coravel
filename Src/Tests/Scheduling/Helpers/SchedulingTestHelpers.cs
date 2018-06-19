using System;
using Coravel.Scheduling.Schedule;

namespace Tests.Scheduling.Helpers
{
    public static class SchedulingTestHelpers
    {
        public static void RunScheduledTasksFromMinutes(Scheduler scheduler, int minutes){
            scheduler.RunScheduledTasks(DateTime.Today.Add(TimeSpan.FromMinutes(minutes)));
        }

        public static void RunScheduledTasksFromDayHourMinutes(Scheduler scheduler, int days, int hours, int minutes){
            var daysSpan = TimeSpan.FromDays(days);
            var hoursSpan = TimeSpan.FromHours(hours);
            var minutesSpan = TimeSpan.FromMinutes(minutes);

            var combinedTimeSpan = daysSpan.Add(hoursSpan).Add(minutesSpan);

            scheduler.RunScheduledTasks(DateTime.Today.Add(combinedTimeSpan));
        }
    }
}