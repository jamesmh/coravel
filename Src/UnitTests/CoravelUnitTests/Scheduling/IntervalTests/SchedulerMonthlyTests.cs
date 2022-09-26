using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using UnitTests.Scheduling.Stubs;
using Xunit;
using static UnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace UnitTests.Scheduling.IntervalTests
{
    public class SchedulerMonthlyTests
    {
        [Theory]
        // Should run
        [InlineData("2018-1-1 00:00:00 am", true)]
        [InlineData("2018-2-1 00:00:00 am", true)]
        [InlineData("2018-3-1 00:00:00 am", true)]
        [InlineData("2018-4-1 00:00:00 am", true)]
        [InlineData("2018-5-1 00:00:00 am", true)]
        [InlineData("2018-6-1 00:00:00 am", true)]
        [InlineData("2018-7-1 00:00:00 am", true)]
        [InlineData("2018-8-1 00:00:00 am", true)]
        [InlineData("2018-9-1 00:00:00 am", true)]
        [InlineData("2018-10-1 00:00:00 am", true)]
        [InlineData("2018-11-1 00:00:00 am", true)]
        [InlineData("2018-12-1 00:00:00 am", true)]

        // Should not run
        [InlineData("2018-7-31 00:00:00 am", false)]
        [InlineData("2018-8-1 00:00:01 am", false)]
        [InlineData("2018-8-2 00:00:00 am", false)]
        [InlineData("2018-8-3 00:00:00 am", false)]
        [InlineData("2018-8-4 00:00:00 am", false)]
        [InlineData("2018-8-5 00:00:00 am", false)]
        [InlineData("2018-8-6 00:00:00 am", false)]
        [InlineData("2018-8-7 00:00:00 am", false)]
        [InlineData("2018-8-8 00:00:00 am", false)]
        [InlineData("2018-8-9 00:01:00 am", false)]
        [InlineData("2018-8-10 01:00:00 am", false)]
        [InlineData("2018-8-31 12:59:59 pm", false)]
        [InlineData("2018-9-1 12:59:59 pm", false)]

        public async Task ValidMonthly(string dateString, bool shouldRun)
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
            bool taskRan = false;

            scheduler.Schedule(() => taskRan = true).Monthly();
            await scheduler.RunAtAsync(DateTime.Parse(dateString));
            Assert.Equal(shouldRun, taskRan);
        }
    }
}