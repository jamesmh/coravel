using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Scheduling.Helpers.SchedulingTestHelpers;

namespace Tests.Scheduling
{
    [TestClass]
    public class SchedulerEveryMinuteTests
    {
        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(15)]
        [DataRow(30)]
        [DataRow(59)]
        public async Task ValidEveryMinute(int minutes)
        {
            var scheduler = new Scheduler();
            bool wasRun = false;

            scheduler.Schedule(() => wasRun = true).EveryMinute();
            await scheduler.RunAtAsync(DateTime.Today.Add(TimeSpan.FromMinutes(minutes)));

            Assert.IsTrue(wasRun);
        }
    }
}