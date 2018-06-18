using System;
using Coravel.Scheduling.Schedule;

namespace Tests.Scheduling
{
    public static class SchedulingTestHelpers
    {
        public static void RunScheduledTasksFromMinutes(Scheduler scheduler, int minutes){
            scheduler.RunScheduledTasks(DateTime.Today.Add(TimeSpan.FromMinutes(minutes)));
        }
    }
}