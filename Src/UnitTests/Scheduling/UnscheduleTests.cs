using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using UnitTests.Scheduling.Stubs;
using Xunit;

namespace UnitTests.Scheduling
{
    public class UnscheduleTests
    {
        [Fact]
        public async Task TestCanUnschedule()
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), null);

            scheduler.ScheduleAsync(async () => await Task.Delay(5))
                .EveryMinute()
                .PreventOverlapping("1");

            scheduler.ScheduleAsync(async () => await Task.Delay(5))
                .EveryMinute()
                .PreventOverlapping("2");

            scheduler.ScheduleAsync(async () => await Task.Delay(5))
                .EveryMinute()
                .PreventOverlapping("3");

            scheduler.ScheduleAsync(async () => await Task.Delay(5))
                .EveryMinute()
                .PreventOverlapping("4");

            scheduler.ScheduleAsync(async () => await Task.Delay(5))
                .EveryMinute()
                .PreventOverlapping("5");

            scheduler.ScheduleAsync(async () => await Task.Delay(5))
                .EveryMinute()
                .PreventOverlapping("6");

            var task = scheduler.RunAtAsync(DateTime.Parse("2018/06/07"));

            bool fiveRemoved = false;
            bool fourRemoved = false;
            bool threeRemoved = false;
            bool twoRemoved = false;

            var task2 = Task.Run(() => fiveRemoved = scheduler.TryUnschedule("5"));
            var task3 = Task.Run(() => fourRemoved = scheduler.TryUnschedule("4"));            
            var task4 = Task.Run(() => threeRemoved = scheduler.TryUnschedule("3"));
            var task5 = Task.Run(() => twoRemoved = scheduler.TryUnschedule("2"));     

            await Task.WhenAll(task, task2, task3, task4, task5);

            Assert.True(fiveRemoved);
            Assert.True(fourRemoved);
            Assert.True(threeRemoved);
            Assert.True(twoRemoved);
        }
    }
}