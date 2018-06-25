using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Scheduling.Helpers.SchedulingTestHelpers;

namespace Tests.Scheduling.RestrictionTests
{
    [TestClass]
    public class SchedulerWeekdays
    {
        [TestMethod]
        [DataTestMethod]
        public async Task DailyOnWeekdaysOnly()
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++)
            .Daily()
            .Weekday();

            await scheduler.RunAtAsync(DateTime.Parse("2018/06/09")); //Sat
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/10")); //Sun
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/11")); //Mon
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/12")); //Tue
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/13")); //W
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/14")); //T
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/15")); //F
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/16")); //S
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/17")); //S
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/18")); //M

            Assert.IsTrue(taskRunCount == 6);
        }
    }
}