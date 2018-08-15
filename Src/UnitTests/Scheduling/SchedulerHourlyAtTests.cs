using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Xunit;
using static UnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace UnitTests.Scheduling
{
    public class SchedulerHourlyAtTests
    {
        [Theory]
        [InlineData(-1, 60)] 
        [InlineData(10, 120)]
        [InlineData(60, 121)]
        [InlineData(0, 63)]
        public async Task ValidHourly_RunOnceOnTheHour(int first, int second)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).HourlyAt(0);

            await RunScheduledTasksFromMinutes(scheduler, first);
            await RunScheduledTasksFromMinutes(scheduler, second);

            Assert.True(taskRunCount == 1);
        }

        [Theory]
        [InlineData(0, 23)] 
        [InlineData(10, 83)]
        [InlineData(60, 143)]
        public async Task ValidHourly_RunOnceAtMin23(int first, int second)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).HourlyAt(23);

            await RunScheduledTasksFromMinutes(scheduler, first);
            await RunScheduledTasksFromMinutes(scheduler, second);

            Assert.True(taskRunCount == 1);
        }

        [Theory]
        [InlineData(1, 0, 70)] 
        [InlineData(5, 4, 66)]
        [InlineData(2, 60, 63)]
        [InlineData(2, 1, 3)]
        public async Task HourlyAt_ShouldNotExecuteScheduleTasks(int runAt, int first, int second)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).HourlyAt(runAt);

            await RunScheduledTasksFromMinutes(scheduler, first);
            await RunScheduledTasksFromMinutes(scheduler, second);

            Assert.True(taskRunCount == 0);
        }
    }
}