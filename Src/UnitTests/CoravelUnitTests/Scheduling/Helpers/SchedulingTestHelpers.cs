using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;

namespace CoravelUnitTests.Scheduling.Helpers;

public static class SchedulingTestHelpers
{
    public static async Task RunScheduledTasksFromMinutes(Scheduler scheduler, int minutes){
        await scheduler.RunAtAsync(DateTime.Today.Add(TimeSpan.FromMinutes(minutes)));
    }

    public static async Task RunScheduledTasksFromDayHourMinutes(Scheduler scheduler, int days, int hours, int minutes){
        var daysSpan = TimeSpan.FromDays(days);
        var hoursSpan = TimeSpan.FromHours(hours);
        var minutesSpan = TimeSpan.FromMinutes(minutes);

        var combinedTimeSpan = daysSpan.Add(hoursSpan).Add(minutesSpan);

        await scheduler.RunAtAsync(DateTime.Today.Add(combinedTimeSpan));
    }
}