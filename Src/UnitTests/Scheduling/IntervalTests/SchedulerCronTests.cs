using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using UnitTests.Scheduling.Stubs;
using Xunit;

namespace UnitTests.Scheduling.IntervalTests
{
    public class SchedulerCronTests
    {
        [Theory]
        // Always
        [InlineData("* * * * *", "1/1/2018 12:00 am", true)]
        [InlineData("* * * * *", "1/1/2018 12:01 am", true)]
        [InlineData("* * * * *", "1/1/2018 1:59 am", true)]
        [InlineData("* * * * *", "1/1/2018 6:22 pm", true)]
        // Minutes
        [InlineData("05 * * * *", "1/1/2018 6:04 pm", false)]
        [InlineData("05 * * * *", "1/1/2018 6:05 pm", true)]
        [InlineData("05 * * * *", "1/1/2018 6:06 pm", false)]
        [InlineData("05,07 * * * *", "1/1/2018 1:05 am", true)]
        [InlineData("05,07 * * * *", "1/1/2018 1:06 am", false)]
        [InlineData("05,07 * * * *", "1/1/2018 1:07 am", true)]
        [InlineData("22-25 * * * *", "1/1/2018 5:21 am", false)]
        [InlineData("22-25 * * * *", "1/1/2018 5:22 am", true)]
        [InlineData("22-25 * * * *", "1/1/2018 5:23 am", true)]
        [InlineData("22-25 * * * *", "1/1/2018 5:25 am", true)]
        [InlineData("22-25 * * * *", "1/1/2018 5:26 am", false)]
        [InlineData("*/5 * * * *", "8/14/2018 5:00 am", true)]
        [InlineData("*/5 * * * *", "8/14/2018 10:05 am", true)]
        [InlineData("*/5 * * * *", "8/14/2018 10:15 am", true)]
        [InlineData("*/5 * * * *", "8/14/2018 5:01 am", false)]
        [InlineData("*/5 * * * *", "8/14/2018 10:14 am", false)]
        [InlineData("*/5 * * * *", "8/14/2018 10:16 am", false)]
        [InlineData("*/2 * * * *", "8/14/2018 2:00 am", true)]
        [InlineData("*/2 * * * *", "8/14/2018 4:02 am", true)]
        [InlineData("*/2 * * * *", "8/14/2018 10:54 am", true)]
        [InlineData("*/2 * * * *", "8/14/2018 12:00 am", true)]
        [InlineData("*/2 * * * *", "8/14/2018 2:01 am", false)]
        [InlineData("*/2 * * * *", "8/14/2018 4:03 am", false)]
        [InlineData("*/2 * * * *", "8/14/2018 10:55 am", false)]
        [InlineData("*/2 * * * *", "8/14/2018 12:13 am", false)]
        // Hours
        [InlineData("* 05 * * *", "1/1/2018 4:04 am", false)]
        [InlineData("* 05 * * *", "1/1/2018 5:05 am", true)]
        [InlineData("* 5 * * *", "1/1/2018 6:00 pm", false)]
        [InlineData("* 5-7 * * *", "1/1/2018 5:33 am", true)]
        [InlineData("* 5-7 * * *", "1/1/2018 6:33 am", true)]
        [InlineData("* 5-7 * * *", "1/1/2018 7:33 am", true)]
        [InlineData("* 20,22 * * *", "1/1/2018 11:00 pm", false)]
        [InlineData("* 20,22 * * *", "1/1/2018 11:59 pm", false)]
        [InlineData("* 20,22 * * *", "1/1/2018 10:59 pm", true)]
        [InlineData("* */2 * * *", "1/1/2018 4:04 am", true)]
        [InlineData("* */2 * * *", "1/1/2018 10:04 am", true)]
        [InlineData("* */2 * * *", "1/1/2018 12:04 pm", true)]
        [InlineData("* */2 * * *", "1/1/2018 5:04 am", false)]
        [InlineData("* */2 * * *", "1/1/2018 11:04 am", false)]
        [InlineData("* */2 * * *", "1/1/2018 9:04 pm", false)]
        [InlineData("* */3 * * *", "1/1/2018 3:04 am", true)]
        [InlineData("* */3 * * *", "1/1/2018 9:04 am", true)]
        [InlineData("* */3 * * *", "1/1/2018 12:04 pm", true)]
        [InlineData("* */3 * * *", "1/1/2018 1:04 am", false)]
        [InlineData("* */3 * * *", "1/1/2018 5:04 am", false)]
        [InlineData("* */3 * * *", "1/1/2018 11:04 pm", false)]
        // Day
        [InlineData("* * 11 * *", "1/11/2018 10:59 pm", true)]
        [InlineData("* * 11 * *", "1/12/2018 10:59 pm", false)]
        [InlineData("* * 11,13,15 * *", "1/11/2018 10:59 pm", true)]
        [InlineData("* * 11,13,15 * *", "1/12/2018 10:59 pm", false)]
        [InlineData("* * 11,13,15 * *", "1/13/2018 10:59 pm", true)]
        [InlineData("* * 11,13,15 * *", "1/14/2018 10:59 pm", false)]
        [InlineData("* * 11,13,15 * *", "1/15/2018 10:59 pm", true)]
        [InlineData("* * 25-27 * *", "1/24/2018 10:59 pm", false)]
        [InlineData("* * 25-27 * *", "1/25/2018 10:59 pm", true)]
        [InlineData("* * 25-27 * *", "1/26/2018 10:59 pm", true)]
        [InlineData("* * 25-27 * *", "1/27/2018 10:59 pm", true)]
        [InlineData("* * 25-27 * *", "1/28/2018 10:59 pm", false)]
        // Month
        [InlineData("* * * 5 *", "4/25/2018 10:59 pm", false)]
        [InlineData("* * * 5 *", "5/25/2018 10:59 pm", true)]
        [InlineData("* * * 5 *", "6/25/2018 10:59 pm", false)]
        [InlineData("* * * 5-7 *", "4/25/2018 10:59 pm", false)]
        [InlineData("* * * 5-7 *", "5/25/2018 10:59 pm", true)]
        [InlineData("* * * 5-7 *", "6/25/2018 10:59 pm", true)]
        [InlineData("* * * 5-7 *", "7/25/2018 10:59 pm", true)]
        [InlineData("* * * 5-7 *", "8/25/2018 10:59 pm", false)]
        [InlineData("* * * 7,11 *", "6/25/2018 10:59 pm", false)]
        [InlineData("* * * 7,11 *", "7/25/2018 10:59 pm", true)]
        [InlineData("* * * 7,11 *", "8/25/2018 10:59 pm", false)]
        [InlineData("* * * 7,11 *", "10/25/2018 10:59 pm", false)]
        [InlineData("* * * 7,11 *", "11/25/2018 10:59 pm", true)]
        [InlineData("* * * 7,11 *", "12/25/2018 10:59 pm", false)]
        [InlineData("* * * */5 *", "5/25/2018 10:59 pm", true)]
        [InlineData("* * * */5 *", "10/25/2018 10:59 pm", true)]
        [InlineData("* * * */5 *", "4/25/2018 10:59 pm", false)]
        [InlineData("* * * */5 *", "11/25/2018 10:59 pm", false)]
        // Week Day
        [InlineData("* * * * 0", "8/12/2018 10:59 pm", true)]
        [InlineData("* * * * 0", "8/13/2018 10:59 pm", false)]
        [InlineData("* * * * 0", "8/18/2018 10:59 pm", false)]
        [InlineData("* * * * 0", "8/19/2018 10:59 pm", true)]
        [InlineData("* * * * 0,6", "8/12/2018 10:59 pm", true)]
        [InlineData("* * * * 0,6", "8/13/2018 10:59 pm", false)]
        [InlineData("* * * * 0,6", "8/18/2018 10:59 pm", true)]
        [InlineData("* * * * 2-4", "8/14/2018 10:59 pm", true)]
        [InlineData("* * * * 2-4", "8/15/2018 10:59 pm", true)]
        [InlineData("* * * * 2-4", "8/16/2018 10:59 pm", true)]
        [InlineData("* * * * 2-4", "8/17/2018 10:59 pm", false)]
        // Mixed
        [InlineData("00 01 * * *", "8/14/2018 1:00 am", true)]
        [InlineData("00 01 * * *", "8/14/2018 1:01 am", false)]
        [InlineData("05 01 * * *", "8/14/2018 1:05 am", true)]
        [InlineData("05 01 * * *", "8/14/2018 1:06 am", false)]
        [InlineData("05 01 * * *", "8/14/2018 1:05 pm", false)]
        [InlineData("00 00 01 * *", "8/1/2018 12:00 am", true)]
        [InlineData("00 00 01 * *", "8/2/2018 12:00 am", false)]
        [InlineData("00 00 01 02 *", "8/1/2018 12:00 am", false)]
        [InlineData("00 00 01 02 *", "2/1/2018 12:00 am", true)]
        [InlineData("00 00 * * 0", "8/19/2018 12:00 am", true)]
        [InlineData("00 00 * * 0", "8/20/2018 12:00 am", false)]
        [InlineData("00 00 */2 * *", "8/2/2018 12:00 am", true)]
        [InlineData("00 00 */2 * *", "8/12/2018 12:00 am", true)]
        [InlineData("00 00 */2 * *", "8/3/2018 12:00 am", false)]
        [InlineData("00 00 */2 * *", "8/13/2018 12:00 am", false)]
        public async Task ScheduledEventCron(string cronExpression, string dateString, bool shouldRun)
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub());
            bool taskRan = false;

            scheduler.Schedule(() => taskRan = true).Cron(cronExpression);

            await scheduler.RunAtAsync(DateTime.Parse(dateString));

            Assert.Equal(shouldRun, taskRan);
        }
    }
}