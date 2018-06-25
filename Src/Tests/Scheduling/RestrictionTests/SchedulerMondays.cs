using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Scheduling.Helpers.SchedulingTestHelpers;

namespace Tests.Scheduling.RestrictionTests
{
    [TestClass]
    public class SchedulerMondays
    {
        [TestMethod]
        [DataTestMethod]
        public async Task DailyOnMondaysOnly() {
              var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++)
            .Daily()
            .Monday();

            await scheduler.RunAtAsync(DateTime.Parse("2018/06/04")); //Monday
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/05"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/06"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/11")); //Monday
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/12"));

            Assert.IsTrue(taskRunCount == 2);
        }
    }
}