using System;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Invocable;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using CoravelUnitTests.Scheduling.Stubs;
using Xunit;

namespace CoravelUnitTests.Scheduling
{
    public class SchedulerGetSchedulesTests
    {
        public class TestInvocable : IInvocable
        {
            public Task Invoke()
            {
                return Task.CompletedTask;
            }
        }

        [Fact]
        public void GetSchedules_ReturnsEmptyList_WhenNoSchedulesExist()
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());

            var schedules = scheduler.GetSchedules();

            Assert.Empty(schedules);
        }

        [Fact]
        public void GetSchedules_ReturnsCorrectData_ForBasicSchedule()
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
            
            scheduler.Schedule(() => { }).Daily();

            var schedules = scheduler.GetSchedules();

            Assert.Single(schedules);
            var schedule = schedules.First();
            Assert.Equal("00 00 * * *", schedule.CronExpression);
            Assert.False(schedule.IsScheduledPerSecond);
            Assert.Null(schedule.SecondsInterval);
            Assert.Null(schedule.InvocableType);
            Assert.False(schedule.PreventOverlapping);
            Assert.NotNull(schedule.EventUniqueId);
            Assert.False(schedule.HasWhenPredicates);
            Assert.Equal(TimeZoneInfo.Utc, schedule.ZonedTimeZone);
            Assert.False(schedule.RunOnceAtStart);
            Assert.False(schedule.RunOnce);
        }

        [Fact]
        public void GetSchedules_ReturnsCorrectData_ForInvocableSchedule()
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
            
            scheduler.Schedule<TestInvocable>().Hourly();

            var schedules = scheduler.GetSchedules();

            Assert.Single(schedules);
            var schedule = schedules.First();
            Assert.Equal("00 * * * *", schedule.CronExpression);
            Assert.False(schedule.IsScheduledPerSecond);
            Assert.Null(schedule.SecondsInterval);
            Assert.Equal(typeof(TestInvocable), schedule.InvocableType);
            Assert.False(schedule.PreventOverlapping);
            Assert.NotNull(schedule.EventUniqueId);
            Assert.False(schedule.HasWhenPredicates);
            Assert.Equal(TimeZoneInfo.Utc, schedule.ZonedTimeZone);
            Assert.False(schedule.RunOnceAtStart);
            Assert.False(schedule.RunOnce);
        }

        [Fact]
        public void GetSchedules_ReturnsCorrectData_ForEverySecondsSchedule()
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
            
            scheduler.Schedule(() => { }).EverySeconds(5);

            var schedules = scheduler.GetSchedules();

            Assert.Single(schedules);
            var schedule = schedules.First();
            Assert.Equal("* * * * *", schedule.CronExpression);
            Assert.True(schedule.IsScheduledPerSecond);
            Assert.Equal(5, schedule.SecondsInterval);
            Assert.Null(schedule.InvocableType);
            Assert.False(schedule.PreventOverlapping);
            Assert.NotNull(schedule.EventUniqueId);
            Assert.False(schedule.HasWhenPredicates);
            Assert.Equal(TimeZoneInfo.Utc, schedule.ZonedTimeZone);
            Assert.False(schedule.RunOnceAtStart);
            Assert.False(schedule.RunOnce);
        }

        [Fact]
        public void GetSchedules_ReturnsCorrectData_ForScheduleWithConfigurations()
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
            
            scheduler.Schedule(() => { })
                .Daily()
                .PreventOverlapping("test-unique-id")
                .When(() => Task.FromResult(true))
                .RunOnceAtStart()
                .Once();

            var schedules = scheduler.GetSchedules();

            Assert.Single(schedules);
            var schedule = schedules.First();
            Assert.Equal("00 00 * * *", schedule.CronExpression);
            Assert.False(schedule.IsScheduledPerSecond);
            Assert.Null(schedule.SecondsInterval);
            Assert.Null(schedule.InvocableType);
            Assert.True(schedule.PreventOverlapping);
            Assert.Equal("test-unique-id", schedule.EventUniqueId);
            Assert.True(schedule.HasWhenPredicates);
            Assert.Equal(TimeZoneInfo.Utc, schedule.ZonedTimeZone);
            Assert.True(schedule.RunOnceAtStart);
            Assert.True(schedule.RunOnce);
        }

        [Fact]
        public void GetSchedules_ReturnsCorrectData_ForZonedSchedule()
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
            var easternTz = TimeZoneInfo.CreateCustomTimeZone("EST", TimeSpan.FromHours(-5), "Eastern Standard Time", "EST");
            
            scheduler.Schedule(() => { })
                .Daily()
                .Zoned(easternTz);

            var schedules = scheduler.GetSchedules();

            Assert.Single(schedules);
            var schedule = schedules.First();
            Assert.Equal(easternTz, schedule.ZonedTimeZone);
        }

        [Fact]
        public void GetSchedules_ReturnsCorrectData_ForCronSchedule()
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
            
            scheduler.Schedule(() => { }).Cron("15 8 * * 1");

            var schedules = scheduler.GetSchedules();

            Assert.Single(schedules);
            var schedule = schedules.First();
            Assert.Equal("15 8 * * 1", schedule.CronExpression);
            Assert.False(schedule.IsScheduledPerSecond);
            Assert.Null(schedule.SecondsInterval);
        }

        [Fact]
        public void GetSchedules_ReturnsMultipleSchedules_WhenMultipleSchedulesExist()
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
            
            scheduler.Schedule(() => { }).Daily();
            scheduler.Schedule<TestInvocable>().Hourly();
            scheduler.Schedule(() => { }).EverySeconds(10);

            var schedules = scheduler.GetSchedules();

            Assert.Equal(3, schedules.Count);
            
            // Verify we have one daily, one hourly, and one every seconds schedule
            Assert.Contains(schedules, s => s.CronExpression == "00 00 * * *");
            Assert.Contains(schedules, s => s.CronExpression == "00 * * * *");
            Assert.Contains(schedules, s => s.IsScheduledPerSecond && s.SecondsInterval == 10);
        }

        [Fact]
        public void GetSchedules_ReturnsCorrectData_ForAsyncSchedule()
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
            
            scheduler.ScheduleAsync(async () => await Task.Delay(1)).Daily();

            var schedules = scheduler.GetSchedules();

            Assert.Single(schedules);
            var schedule = schedules.First();
            Assert.Equal("00 00 * * *", schedule.CronExpression);
            Assert.False(schedule.IsScheduledPerSecond);
            Assert.Null(schedule.SecondsInterval);
            Assert.Null(schedule.InvocableType);
            Assert.False(schedule.PreventOverlapping);
            Assert.NotNull(schedule.EventUniqueId);
            Assert.False(schedule.HasWhenPredicates);
            Assert.Equal(TimeZoneInfo.Utc, schedule.ZonedTimeZone);
            Assert.False(schedule.RunOnceAtStart);
            Assert.False(schedule.RunOnce);
        }
    }
}