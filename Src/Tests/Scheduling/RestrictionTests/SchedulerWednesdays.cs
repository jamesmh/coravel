using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Scheduling.Helpers.SchedulingTestHelpers;

namespace Tests.Scheduling.RestrictionTests
{
    [TestClass]
    public class SchedulerWednesdays
    {
        [TestMethod]
        [DataTestMethod]
        public async Task DailyOnWednesdayOnly() {
              var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++)
            .Daily()
            .Wednesday();

           await scheduler.RunAtAsync(DateTime.Parse("2018/06/05")); 
           await scheduler.RunAtAsync(DateTime.Parse("2018/06/06")); //Wednesday
           await scheduler.RunAtAsync(DateTime.Parse("2018/06/07"));
           await scheduler.RunAtAsync(DateTime.Parse("2018/06/12")); 
           await scheduler.RunAtAsync(DateTime.Parse("2018/06/13")); //Wednesday
           await scheduler.RunAtAsync(DateTime.Parse("2018/06/14")); 

            Assert.IsTrue(taskRunCount == 2);
        }
    }
}