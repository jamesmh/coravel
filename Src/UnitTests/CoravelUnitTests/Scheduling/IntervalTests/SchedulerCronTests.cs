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
    public class SchedulerCronTests
    {
        [Theory]
        // Always
        [InlineData("* * * * *", "1/1/2018 12:00 am", true, false)]
        [InlineData("* * * * *", "1/1/2018 12:01 am", true, false)]
        [InlineData("* * * * *", "1/1/2018 1:59 am", true, false)]
        [InlineData("* * * * *", "1/1/2018 6:22 pm", true, false)]
        // Minutes
        [InlineData("05 * * * *", "1/1/2018 6:04 pm", false, false)]
        [InlineData("05 * * * *", "1/1/2018 6:05 pm", true, false)]
        [InlineData("05 * * * *", "1/1/2018 6:06 pm", false, false)]
        [InlineData("05,07 * * * *", "1/1/2018 1:05 am", true, false)]
        [InlineData("05,07 * * * *", "1/1/2018 1:06 am", false, false)]
        [InlineData("05,07 * * * *", "1/1/2018 1:07 am", true, false)]
        [InlineData("22-25 * * * *", "1/1/2018 5:21 am", false, false)]
        [InlineData("22-25 * * * *", "1/1/2018 5:22 am", true, false)]
        [InlineData("22-25 * * * *", "1/1/2018 5:23 am", true, false)]
        [InlineData("22-25 * * * *", "1/1/2018 5:25 am", true, false)]
        [InlineData("22-25 * * * *", "1/1/2018 5:26 am", false, false)]
        [InlineData("*/5 * * * *", "8/14/2018 5:00 am", true, false)]
        [InlineData("*/5 * * * *", "8/14/2018 10:05 am", true, false)]
        [InlineData("*/5 * * * *", "8/14/2018 10:15 am", true, false)]
        [InlineData("*/5 * * * *", "8/14/2018 5:01 am", false, false)]
        [InlineData("*/5 * * * *", "8/14/2018 10:14 am", false, false)]
        [InlineData("*/5 * * * *", "8/14/2018 10:16 am", false, false)]
        [InlineData("*/2 * * * *", "8/14/2018 2:00 am", true, false)]
        [InlineData("*/2 * * * *", "8/14/2018 4:02 am", true, false)]
        [InlineData("*/2 * * * *", "8/14/2018 10:54 am", true, false)]
        [InlineData("*/2 * * * *", "8/14/2018 12:00 am", true, false)]
        [InlineData("*/2 * * * *", "8/14/2018 2:01 am", false, false)]
        [InlineData("*/2 * * * *", "8/14/2018 4:03 am", false, false)]
        [InlineData("*/2 * * * *", "8/14/2018 10:55 am", false, false)]
        [InlineData("*/2 * * * *", "8/14/2018 12:13 am", false, false)]
        // Hours
        [InlineData("* 05 * * *", "1/1/2018 4:04 am", false, false)]
        [InlineData("* 05 * * *", "1/1/2018 5:05 am", true, false)]
        [InlineData("* 5 * * *", "1/1/2018 6:00 pm", false, false)]
        [InlineData("* 5-7 * * *", "1/1/2018 5:33 am", true, false)]
        [InlineData("* 5-7 * * *", "1/1/2018 6:33 am", true, false)]
        [InlineData("* 5-7 * * *", "1/1/2018 7:33 am", true, false)]
        [InlineData("* 20,22 * * *", "1/1/2018 11:00 pm", false, false)]
        [InlineData("* 20,22 * * *", "1/1/2018 11:59 pm", false, false)]
        [InlineData("* 20,22 * * *", "1/1/2018 10:59 pm", true, false)]
        [InlineData("* */2 * * *", "1/1/2018 4:04 am", true, false)]
        [InlineData("* */2 * * *", "1/1/2018 10:04 am", true, false)]
        [InlineData("* */2 * * *", "1/1/2018 12:04 pm", true, false)]
        [InlineData("* */2 * * *", "1/1/2018 5:04 am", false, false)]
        [InlineData("* */2 * * *", "1/1/2018 11:04 am", false, false)]
        [InlineData("* */2 * * *", "1/1/2018 9:04 pm", false, false)]
        [InlineData("* */3 * * *", "1/1/2018 3:04 am", true, false)]
        [InlineData("* */3 * * *", "1/1/2018 9:04 am", true, false)]
        [InlineData("* */3 * * *", "1/1/2018 12:04 pm", true, false)]
        [InlineData("* */3 * * *", "1/1/2018 1:04 am", false, false)]
        [InlineData("* */3 * * *", "1/1/2018 5:04 am", false, false)]
        [InlineData("* */3 * * *", "1/1/2018 11:04 pm", false, false)]
        // Day
        [InlineData("* * 11 * *", "1/11/2018 10:59 pm", true, false)]
        [InlineData("* * 11 * *", "1/12/2018 10:59 pm", false, false)]
        [InlineData("* * 11,13,15 * *", "1/11/2018 10:59 pm", true, false)]
        [InlineData("* * 11,13,15 * *", "1/12/2018 10:59 pm", false, false)]
        [InlineData("* * 11,13,15 * *", "1/13/2018 10:59 pm", true, false)]
        [InlineData("* * 11,13,15 * *", "1/14/2018 10:59 pm", false, false)]
        [InlineData("* * 11,13,15 * *", "1/15/2018 10:59 pm", true, false)]
        [InlineData("* * 25-27 * *", "1/24/2018 10:59 pm", false, false)]
        [InlineData("* * 25-27 * *", "1/25/2018 10:59 pm", true, false)]
        [InlineData("* * 25-27 * *", "1/26/2018 10:59 pm", true, false)]
        [InlineData("* * 25-27 * *", "1/27/2018 10:59 pm", true, false)]
        [InlineData("* * 25-27 * *", "1/28/2018 10:59 pm", false, false)]
        // Month
        [InlineData("* * * 5 *", "4/25/2018 10:59 pm", false, false)]
        [InlineData("* * * 5 *", "5/25/2018 10:59 pm", true, false)]
        [InlineData("* * * 5 *", "6/25/2018 10:59 pm", false, false)]
        [InlineData("* * * 5-7 *", "4/25/2018 10:59 pm", false, false)]
        [InlineData("* * * 5-7 *", "5/25/2018 10:59 pm", true, false)]
        [InlineData("* * * 5-7 *", "6/25/2018 10:59 pm", true, false)]
        [InlineData("* * * 5-7 *", "7/25/2018 10:59 pm", true, false)]
        [InlineData("* * * 5-7 *", "8/25/2018 10:59 pm", false, false)]
        [InlineData("* * * 7,11 *", "6/25/2018 10:59 pm", false, false)]
        [InlineData("* * * 7,11 *", "7/25/2018 10:59 pm", true, false)]
        [InlineData("* * * 7,11 *", "8/25/2018 10:59 pm", false, false)]
        [InlineData("* * * 7,11 *", "10/25/2018 10:59 pm", false, false)]
        [InlineData("* * * 7,11 *", "11/25/2018 10:59 pm", true, false)]
        [InlineData("* * * 7,11 *", "12/25/2018 10:59 pm", false, false)]
        [InlineData("* * * */5 *", "5/25/2018 10:59 pm", true, false)]
        [InlineData("* * * */5 *", "10/25/2018 10:59 pm", true, false)]
        [InlineData("* * * */5 *", "4/25/2018 10:59 pm", false, false)]
        [InlineData("* * * */5 *", "11/25/2018 10:59 pm", false, false)]
        // Week Day
        [InlineData("* * * * 0", "8/12/2018 10:59 pm", true, false)]
        [InlineData("* * * * 0", "8/13/2018 10:59 pm", false, false)]
        [InlineData("* * * * 0", "8/18/2018 10:59 pm", false, false)]
        [InlineData("* * * * 0", "8/19/2018 10:59 pm", true, false)]
        [InlineData("* * * * 0,6", "8/12/2018 10:59 pm", true, false)]
        [InlineData("* * * * 0,6", "8/13/2018 10:59 pm", false, false)]
        [InlineData("* * * * 0,6", "8/18/2018 10:59 pm", true, false)]
        [InlineData("* * * * 2-4", "8/14/2018 10:59 pm", true, false)]
        [InlineData("* * * * 2-4", "8/15/2018 10:59 pm", true, false)]
        [InlineData("* * * * 2-4", "8/16/2018 10:59 pm", true, false)]
        [InlineData("* * * * 2-4", "8/17/2018 10:59 pm", false, false)]
        // Mixed
        [InlineData("00 01 * * *", "8/14/2018 1:00 am", true, false)]
        [InlineData("00 01 * * *", "8/14/2018 1:01 am", false, false)]
        [InlineData("05 01 * * *", "8/14/2018 1:05 am", true, false)]
        [InlineData("05 01 * * *", "8/14/2018 1:06 am", false, false)]
        [InlineData("05 01 * * *", "8/14/2018 1:05 pm", false, false)]
        [InlineData("00 00 01 * *", "8/1/2018 12:00 am", true, false)]
        [InlineData("00 00 01 * *", "8/2/2018 12:00 am", false, false)]
        [InlineData("00 00 01 02 *", "8/1/2018 12:00 am", false, false)]
        [InlineData("00 00 01 02 *", "2/1/2018 12:00 am", true, false)]
        [InlineData("00 00 * * 0", "8/19/2018 12:00 am", true, false)]
        [InlineData("00 00 * * 0", "8/20/2018 12:00 am", false, false)]
        [InlineData("00 00 */2 * *", "8/2/2018 12:00 am", true, false)]
        [InlineData("00 00 */2 * *", "8/12/2018 12:00 am", true, false)]
        [InlineData("00 00 */2 * *", "8/3/2018 12:00 am", false, false)]
        [InlineData("00 00 */2 * *", "8/13/2018 12:00 am", false, false)]
        // Mixed, TimeZoneAware
        [InlineData("00 01 * * *", "8/14/2018 12:00 am", true, true)]
        [InlineData("00 01 * * *", "8/14/2018 12:01 am", false, true)]
        [InlineData("05 01 * * *", "8/14/2018 12:05 am", true, true)]
        [InlineData("05 01 * * *", "8/14/2018 12:06 am", false, true)]
        [InlineData("05 01 * * *", "8/14/2018 12:05 pm", false, true)]
        [InlineData("00 00 01 * *", "7/31/2018 11:00 pm", true, true)]
        [InlineData("00 00 01 * *", "8/1/2018 11:00 pm", false, true)]
        [InlineData("00 00 01 02 *", "7/31/2018 11:00 pm", false, true)]
        [InlineData("00 00 01 02 *", "1/31/2018 11:00 pm", true, true)]
        [InlineData("00 00 * * 0", "8/18/2018 11:00 pm", true, true)]
        [InlineData("00 00 * * 0", "8/19/2018 11:00 pm", false, true)]
        [InlineData("00 00 */2 * *", "8/1/2018 11:00 pm", true, true)]
        [InlineData("00 00 */2 * *", "8/11/2018 11:00 pm", true, true)]
        [InlineData("00 00 */2 * *", "8/2/2018 11:00 pm", false, true)]
        [InlineData("00 00 */2 * *", "8/12/2018 11:00 pm", false, true)]
        public async Task ScheduledEventCron(string cronExpression, string dateString, bool shouldRun,
            bool withTimeZone)
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
            bool taskRan = false;

            var timeZoneInfo = withTimeZone ? GetTimeZone() : null;

            scheduler.Schedule(() => taskRan = true).Cron(cronExpression, timeZoneInfo);

            await scheduler.RunAtAsync(
                DateTime.ParseExact(dateString, "M/d/yyyy h:mm tt", CultureInfo.InvariantCulture));

            Assert.Equal(shouldRun, taskRan);
        }

        private static TimeZoneInfo GetTimeZone()
        {
            // generate a fake TimeZone, so that the test works independent on all systems (Windows uses different TimeZoneId's, than *nix)
            return TimeZoneInfo.CreateCustomTimeZone("fake", TimeSpan.FromHours(1), "fake", null, null, null, false);
        }
    }
}