using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using Coravel.Scheduling.Schedule.UtcTime;
using UnitTests.Scheduling.Stubs;
using Xunit;

namespace UnitTests.Scheduling.IntervalTests
{
    public class SchedulerCronWithTimeZoneTests
    {
        [Theory]
        // Mixed, TimeZoneAware, Offset +1
        [InlineData("00 01 * * *", "8/14/2018 12:00 am", true, 1)]
        [InlineData("00 01 * * *", "8/14/2018 12:01 am", false, 1)]
        [InlineData("05 01 * * *", "8/14/2018 12:05 am", true, 1)]
        [InlineData("05 01 * * *", "8/14/2018 12:06 am", false, 1)]
        [InlineData("05 01 * * *", "8/14/2018 12:05 pm", false, 1)]
        [InlineData("00 00 01 * *", "7/31/2018 11:00 pm", true, 1)]
        [InlineData("00 00 01 * *", "8/1/2018 11:00 pm", false, 1)]
        [InlineData("00 00 01 02 *", "7/31/2018 11:00 pm", false, 1)]
        [InlineData("00 00 01 02 *", "1/31/2018 11:00 pm", true, 1)]
        [InlineData("00 00 * * 0", "8/18/2018 11:00 pm", true, 1)]
        [InlineData("00 00 * * 0", "8/19/2018 11:00 pm", false, 1)]
        [InlineData("00 00 */2 * *", "8/1/2018 11:00 pm", true, 1)]
        [InlineData("00 00 */2 * *", "8/11/2018 11:00 pm", true, 1)]
        [InlineData("00 00 */2 * *", "8/2/2018 11:00 pm", false, 1)]
        [InlineData("00 00 */2 * *", "8/12/2018 11:00 pm", false, 1)]
        // Mixed, TimeZoneAware, Offset +2
        [InlineData("00 01 * * *", "8/14/2018 11:00 pm", true, 2)]
        [InlineData("00 01 * * *", "8/14/2018 11:01 pm", false, 2)]
        [InlineData("05 01 * * *", "8/14/2018 11:05 pm", true, 2)]
        [InlineData("05 01 * * *", "8/14/2018 11:06 pm", false, 2)]
        [InlineData("05 01 * * *", "8/14/2018 11:05 am", false, 2)]
        [InlineData("00 00 01 * *", "7/31/2018 10:00 pm", true, 2)]
        [InlineData("00 00 01 * *", "8/1/2018 10:00 pm", false, 2)]
        [InlineData("00 00 01 02 *", "7/31/2018 10:00 pm", false, 2)]
        [InlineData("00 00 01 02 *", "1/31/2018 10:00 pm", true, 2)]
        [InlineData("00 00 * * 0", "8/18/2018 10:00 pm", true, 2)]
        [InlineData("00 00 * * 0", "8/19/2018 10:00 pm", false, 2)]
        [InlineData("00 00 */2 * *", "8/1/2018 10:00 pm", true, 2)]
        [InlineData("00 00 */2 * *", "8/11/2018 10:00 pm", true, 2)]
        [InlineData("00 00 */2 * *", "8/2/2018 10:00 pm", false, 2)]
        [InlineData("00 00 */2 * *", "8/12/2018 10:00 pm", false, 2)]
        public async Task ScheduledEventCron(string cronExpression, string dateString, bool shouldRun, 
            int offset)
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
            bool taskRan = false;

            scheduler.Schedule(() => taskRan = true).CronWithTimeZone(cronExpression, GetTimeZone(offset));

            await scheduler.RunAtAsync(
                DateTime.ParseExact(dateString, "M/d/yyyy h:mm tt", CultureInfo.InvariantCulture));

            Assert.Equal(shouldRun, taskRan);
        }

        private static TimeZoneInfo GetTimeZone(int offset)
        {
            // generate a fake TimeZone, so that the test works independent on all systems (Windows uses different TimeZoneId's, than *nix)
            return TimeZoneInfo.CreateCustomTimeZone("fake", TimeSpan.FromHours(offset), "fake", null, null, null, false);
        }
    }
}