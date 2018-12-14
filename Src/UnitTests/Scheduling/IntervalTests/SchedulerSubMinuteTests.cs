using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Interfaces;
using Coravel.Scheduling.Schedule.Mutex;
using Coravel.Scheduling.Schedule.UtcTime;
using UnitTests.Scheduling.Stubs;
using Xunit;

namespace UnitTests.Scheduling.IntervalTests
{
    public class SchedulerSubMinuteTests
    {
        [Theory]
        [InlineData("1/1/2018 12:00:00 am", true)]
        [InlineData("1/1/2018 12:00:01 am", true)]
        [InlineData("1/1/2018 12:00:02 am", true)]
        [InlineData("1/1/2018 12:00:05 am", true)]
        [InlineData("1/1/2018 12:00:10 am", true)]
        [InlineData("1/1/2018 12:00:15 am", true)]
        [InlineData("1/1/2018 12:00:30 am", true)]
        [InlineData("1/1/2018 12:00:31 am", true)]
        [InlineData("2/15/2020 5:55:00 pm", true)]
        [InlineData("2/15/2020 5:55:04 pm", true)]
        [InlineData("2/15/2020 5:55:05 pm", true)]
        [InlineData("2/15/2020 5:55:06 pm", true)]
        public async Task ScheduledEventPerSecond(string dateString, bool shouldRun)
        {
            await TestSubMinuteInterval(dateString, shouldRun, e => e.EverySecond());
        }

        [Theory]
        [InlineData("1/1/2018 12:00:00 am", true)]
        [InlineData("1/1/2018 12:00:05 am", true)]
        [InlineData("1/1/2018 12:00:10 am", true)]
        [InlineData("1/1/2018 12:00:15 am", true)]
        [InlineData("1/1/2018 12:00:20 pm", true)]
        [InlineData("1/1/2018 12:00:25 pm", true)]
        [InlineData("1/1/2018 12:00:30 pm", true)]
        [InlineData("1/1/2018 12:00:35 pm", true)]
        [InlineData("1/1/2018 12:00:40 am", true)]
        [InlineData("1/1/2018 12:00:45 am", true)]
        [InlineData("1/1/2018 12:00:50 am", true)]
        [InlineData("1/1/2018 12:00:55 am", true)]

        [InlineData("1/1/2018 2:00:01 am", false)]
        [InlineData("1/1/2018 3:00:02 am", false)]
        [InlineData("1/1/2018 4:00:05 am", true)]
        [InlineData("1/1/2018 5:00:10 am", true)]
        [InlineData("1/1/2018 6:00:15 am", true)]
        [InlineData("1/1/2018 7:00:30 am", true)]
        [InlineData("1/1/2018 8:00:31 am", false)]
        [InlineData("2/15/2020 5:55:00 pm", true)]
        [InlineData("2/15/2020 5:55:04 pm", false)]
        [InlineData("2/15/2020 5:55:05 pm", true)]
        [InlineData("2/15/2020 5:55:06 pm", false)]
        public async Task ScheduledEventEveryFiveSeconds(string dateString, bool shouldRun)
        {
            await TestSubMinuteInterval(dateString, shouldRun, e => e.EveryFiveSeconds());
        }

          [Theory]
        [InlineData("1/1/2018 12:00:00 am", true)]
        [InlineData("1/1/2018 12:00:10 am", true)]
        [InlineData("1/1/2018 12:00:20 am", true)]
        [InlineData("1/1/2018 12:00:30 am", true)]
        [InlineData("1/1/2018 12:00:40 pm", true)]
        [InlineData("1/1/2018 12:00:50 pm", true)]
        [InlineData("1/1/2018 1:45:00 pm", true)]
        [InlineData("1/1/2018 1:45:10 pm", true)]
        [InlineData("1/1/2018 1:45:20 pm", true)]
        [InlineData("1/1/2018 1:45:30 pm", true)]
        [InlineData("1/1/2018 1:45:40 pm", true)]
        [InlineData("1/1/2018 1:45:50 pm", true)]

        [InlineData("1/1/2018 2:00:01 am", false)]
        [InlineData("1/1/2018 3:00:02 am", false)]
        [InlineData("1/1/2018 4:00:05 am", false)]
        [InlineData("1/1/2018 5:00:10 am", true)]
        [InlineData("1/1/2018 6:00:15 am", false)]
        [InlineData("1/1/2018 7:00:30 am", true)]
        [InlineData("1/1/2018 8:00:31 am", false)]
        [InlineData("2/15/2020 5:55:00 pm", true)]
        [InlineData("2/15/2020 5:55:04 pm", false)]
        [InlineData("2/15/2020 5:55:05 pm", false)]
        [InlineData("2/15/2020 5:55:06 pm", false)]
        public async Task ScheduledEventEveryTenSeconds(string dateString, bool shouldRun)
        {
            await TestSubMinuteInterval(dateString, shouldRun, e => e.EveryTenSeconds());
        }

        [Theory]
        [InlineData("1/1/2018 12:00:00 am", true)]
        [InlineData("1/1/2018 12:00:15 am", true)]
        [InlineData("1/1/2018 12:00:30 am", true)]
        [InlineData("1/1/2018 12:00:45 am", true)]
        [InlineData("1/1/2018 4:46:00 pm", true)]
        [InlineData("1/1/2018 4:46:15 pm", true)]
        [InlineData("1/1/2018 4:46:30 pm", true)]
        [InlineData("1/1/2018 4:46:45 pm", true)]

        [InlineData("1/1/2018 2:00:01 am", false)]
        [InlineData("1/1/2018 3:00:02 am", false)]
        [InlineData("1/1/2018 4:00:05 am", false)]
        [InlineData("1/1/2018 5:00:10 am", false)]
        [InlineData("1/1/2018 6:00:15 am", true)]
        [InlineData("1/1/2018 7:00:30 am", true)]
        [InlineData("1/1/2018 8:00:31 am", false)]
        [InlineData("2/15/2020 5:55:00 pm", true)]
        [InlineData("2/15/2020 5:55:04 pm", false)]
        [InlineData("2/15/2020 5:55:05 pm", false)]
        [InlineData("2/15/2020 5:55:06 pm", false)]
        public async Task ScheduledEventEveryFifteenSeconds(string dateString, bool shouldRun)
        {
            await TestSubMinuteInterval(dateString, shouldRun, e => e.EveryFifteenSeconds());
        }

                [Theory]
        [InlineData("1/1/2018 12:00:00 am", true)]
        [InlineData("1/1/2018 12:00:30 am", true)]
        [InlineData("1/1/2018 4:46:00 pm", true)]
        [InlineData("1/1/2018 4:46:30 pm", true)]
        [InlineData("1/1/2018 12:01:00 am", true)]
        [InlineData("1/1/2018 12:01:30 am", true)]

        [InlineData("1/1/2018 2:00:01 am", false)]
        [InlineData("1/1/2018 3:00:02 am", false)]
        [InlineData("1/1/2018 4:00:05 am", false)]
        [InlineData("1/1/2018 5:00:10 am", false)]
        [InlineData("1/1/2018 6:00:15 am", false)]
        [InlineData("1/1/2018 7:00:30 am", true)]
        [InlineData("1/1/2018 8:00:31 am", false)]
        [InlineData("2/15/2020 5:55:00 pm", true)]
        [InlineData("2/15/2020 5:55:04 pm", false)]
        [InlineData("2/15/2020 5:55:05 pm", false)]
        [InlineData("2/15/2020 5:55:06 pm", false)]
        public async Task ScheduledEventEveryThirtySeconds(string dateString, bool shouldRun)
        {
            await TestSubMinuteInterval(dateString, shouldRun, e => e.EveryThirtySeconds());
        }

        private static async Task TestSubMinuteInterval(string dateString, bool shouldRun, Action<IScheduleInterval> scheduleIt)
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
            int taskRunCount = 0;
            Action run = () => Interlocked.Increment(ref taskRunCount);
            scheduleIt(scheduler.Schedule(run));
            await scheduler.RunAtAsync(DateTime.ParseExact(dateString, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture));
            Assert.Equal(shouldRun, taskRunCount == 1);
        }
    }
}