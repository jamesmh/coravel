using System;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Scheduling.SchedulingTestHelpers;

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
        public void ValidEveryMinute(int minutes)
        {
            var scheduler = new Scheduler();
            bool wasRun = false;

            scheduler.Schedule(() => wasRun = true).EveryMinute();
            scheduler.RunScheduledTasks(DateTime.Today.Add(TimeSpan.FromMinutes(minutes)));

            Assert.IsTrue(wasRun);
        }
    }
}