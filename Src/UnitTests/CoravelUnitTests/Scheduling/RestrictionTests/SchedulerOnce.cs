using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using CoravelUnitTests.Scheduling.Stubs;
using Xunit;

namespace CoravelUnitTests.Scheduling.RestrictionTests
{
    public class SchedulerOnce
    {
        [Fact]
        public async Task ScheduleOnceWithCron()
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++)
                .Cron("* * * * *")
                .Once();

            await scheduler.RunAtAsync(DateTime.Parse("2018/06/09"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/10"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/11"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/12"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/13"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/14"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/15"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/16"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/17"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/18"));

            Assert.True(taskRunCount == 1);
        }
        
        [Fact]
        public async Task ScheduleOnceAtHourly()
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++)
               .Hourly()
                .Once();
            
            // Shouldn't be due here.
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/09 12:00:01 pm"));
            Assert.True(taskRunCount == 0);

            await scheduler.RunAtAsync(DateTime.Parse("2018/06/09 12:00:00 pm"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/10 5:00:00 am"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/09 1:00:00 am"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/10 2:00:00 pm"));

            Assert.True(taskRunCount == 1);
        }
    }
}