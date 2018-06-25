using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Scheduling.Helpers.SchedulingTestHelpers;

namespace Tests.Scheduling.RestrictionTests
{
    [TestClass]
    public class SchedulerTuesdays
    {
        [TestMethod]
        [DataTestMethod]
        public async Task DailyOnTuesdaysOnly() {
              var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++)
            .Daily()
            .Tuesday();

            await scheduler.RunAtAsync(DateTime.Parse("2018/06/05")); //Tuesday
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/06"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/07"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/12")); //Tuesday
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/13"));

            Assert.IsTrue(taskRunCount == 2);
        }
    }
}