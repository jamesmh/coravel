using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Scheduling.Helpers.SchedulingTestHelpers;

namespace Tests.Scheduling.RestrictionTests
{
    [TestClass]
    public class SchedulerFridays
    {
        [TestMethod]
        [DataTestMethod]
        public async Task DailyOnFridaysOnly() {
              var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++)
            .Daily()
            .Friday();

            await scheduler.RunAtAsync(DateTime.Parse("2018/06/07")); 
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/08")); //Friday
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/09"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/14")); 
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/15")); //Friday
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/16")); 

            Assert.IsTrue(taskRunCount == 2);
        }
    }
}