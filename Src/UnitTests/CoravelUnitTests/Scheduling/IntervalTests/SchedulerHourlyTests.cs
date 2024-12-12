using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using CoravelUnitTests.Scheduling.Stubs;
using Xunit;
using static CoravelUnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace CoravelUnitTests.Scheduling.IntervalTests
{
    public class SchedulerHourlyTests
    {
        [Theory]
        // Should run
        [InlineData(0, 0, 0, true)]
        [InlineData(1, 4, 0, true)]
        [InlineData(2, 8, 0, true)]
        [InlineData(3, 13, 0, true)]
        [InlineData(4, 21, 0, true)]
        [InlineData(5, 23, 0, true)]
        [InlineData(6, 1, 0, true)]
        [InlineData(7, 1, 0, true, "th-TH")]
        // Should not run
        [InlineData(0, 0, 1, false)]
        [InlineData(1, 4, 12, false)]
        [InlineData(2, 8, 30, false)]
        [InlineData(3, 13, 33, false)]
        [InlineData(4, 21, 59, false)]
        [InlineData(5, 23, 58, false)]
        [InlineData(6, 1, 44, false)]
        [InlineData(0, 0, 55, false)]
        [InlineData(1, 4, 31, false)]
        [InlineData(2, 8, 12, false)]
        [InlineData(3, 13, 1, false)]
        [InlineData(4, 21, 2, false)]
        [InlineData(5, 23, 3, false)]
        [InlineData(6, 1, 4, false)]
        [InlineData(0, 0, 15, false)]
        [InlineData(1, 4, 20, false)]
        [InlineData(2, 8, 25, false)]
        [InlineData(3, 13, 30, false)]
        [InlineData(4, 21, 45, false)]
        [InlineData(5, 23, 55, false)]
        [InlineData(6, 1, 1, false)]
        [InlineData(7, 1, 1, false, "th-TH")]   // Try with different culture since it affects IndexOf calls

        public async Task ValidHourly(int day, int hour, int minute, bool shouldRun, string culture = null)
        {
            // Remember current culture in order to clean up
            var prevCulture = Thread.CurrentThread.CurrentCulture;

            try
            {
                // Set culture if needed
                if (culture != null)
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(culture, false);

                var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
                bool taskRan = false;

                scheduler.Schedule(() => taskRan = true).Hourly();

                await RunScheduledTasksFromDayHourMinutes(scheduler, day, hour, minute);

                Assert.Equal(shouldRun, taskRan);
            }
            finally
            {
                // Revert to previous culture if it has been changed
                if (culture != null)
                    Thread.CurrentThread.CurrentCulture = prevCulture;
            }
        }
    }
}