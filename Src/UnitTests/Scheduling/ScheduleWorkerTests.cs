using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using UnitTests.Scheduling.Stubs;
using Xunit;

namespace UnitTests.Scheduling
{
    public class ScheduleWorkerTests
    {
        [Fact]
        public async Task ScheduleWorkerSuccessfulWithOneThreadPerWorker()
        {
            for (int i = 0; i < 100; i++)
            {
                var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), null);

                List<int> threads1 = new List<int>();
                List<int> threads2 = new List<int>();
                List<int> threads3 = new List<int>();
                List<int> threads4 = new List<int>();
                List<int> threads5 = new List<int>();
                List<int> threads6 = new List<int>();
                List<int> threads7 = new List<int>();
                List<int> threads8 = new List<int>();

                Action action1 = () =>
                {
                    threads1.Add(Thread.CurrentThread.ManagedThreadId);
                };
                Action action2 = () =>
                {
                    threads2.Add(Thread.CurrentThread.ManagedThreadId);
                };
                Action action3 = () =>
                {
                    threads3.Add(Thread.CurrentThread.ManagedThreadId);
                };
                Action action4 = () =>
                {
                    threads4.Add(Thread.CurrentThread.ManagedThreadId);
                };
                Action action5 = () =>
                {
                    threads5.Add(Thread.CurrentThread.ManagedThreadId);
                };
                Action action6 = () =>
                {
                    threads6.Add(Thread.CurrentThread.ManagedThreadId);
                };
                Action action7 = () =>
                {
                    threads7.Add(Thread.CurrentThread.ManagedThreadId);
                };
                Action action8 = () =>
                {
                    threads8.Add(Thread.CurrentThread.ManagedThreadId);
                };

                scheduler.OnWorker("worker1");
                scheduler.Schedule(action1).EveryMinute();

                scheduler.OnWorker("worker2");
                scheduler.Schedule(action2).EveryMinute();
                scheduler.Schedule(action2).EveryMinute();

                scheduler.OnWorker("worker3");
                scheduler.Schedule(action3).EveryMinute();
                scheduler.Schedule(action3).EveryMinute();
                scheduler.Schedule(action3).EveryMinute();

                scheduler.OnWorker("worker4");
                scheduler.Schedule(action4).EveryMinute();
                scheduler.Schedule(action4).EveryMinute();
                scheduler.Schedule(action4).EveryMinute();
                scheduler.Schedule(action4).EveryMinute();

                scheduler.OnWorker("worker5");
                scheduler.Schedule(action5).EveryMinute();
                scheduler.Schedule(action5).EveryMinute();
                scheduler.Schedule(action5).EveryMinute();
                scheduler.Schedule(action5).EveryMinute();
                scheduler.Schedule(action5).EveryMinute();

                scheduler.OnWorker("worker6");
                scheduler.Schedule(action6).EveryMinute();
                scheduler.Schedule(action6).EveryMinute();
                scheduler.Schedule(action6).EveryMinute();
                scheduler.Schedule(action6).EveryMinute();
                scheduler.Schedule(action6).EveryMinute();
                scheduler.Schedule(action6).EveryMinute();

                scheduler.OnWorker("worker7");
                scheduler.Schedule(action7).EveryMinute();
                scheduler.Schedule(action7).EveryMinute();
                scheduler.Schedule(action7).EveryMinute();
                scheduler.Schedule(action7).EveryMinute();
                scheduler.Schedule(action7).EveryMinute();
                scheduler.Schedule(action7).EveryMinute();
                scheduler.Schedule(action7).EveryMinute();

                scheduler.OnWorker("worker8");
                scheduler.Schedule(action8).EveryMinute();
                scheduler.Schedule(action8).EveryMinute();
                scheduler.Schedule(action8).EveryMinute();
                scheduler.Schedule(action8).EveryMinute();
                scheduler.Schedule(action8).EveryMinute();
                scheduler.Schedule(action8).EveryMinute();
                scheduler.Schedule(action8).EveryMinute();
                scheduler.Schedule(action8).EveryMinute();

                await scheduler.RunAtAsync(DateTime.Parse("2018/06/07"));

                // Make sure for each worker only one thread was used (synchronously)
                Assert.True(threads1.Distinct().Count() == 1);
                Assert.True(threads2.Distinct().Count() == 1);
                Assert.True(threads3.Distinct().Count() == 1);
                Assert.True(threads4.Distinct().Count() == 1);
                Assert.True(threads5.Distinct().Count() == 1);
                Assert.True(threads6.Distinct().Count() == 1);
                Assert.True(threads7.Distinct().Count() == 1);
                Assert.True(threads8.Distinct().Count() == 1);
            }
        }

        [Fact]
        public async Task ScheduleWorkerSuccessfulOnMultipleThreadsWithMutiRuns()
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), null);

            int counter1 = 0;
            int counter2 = 0;
            int counter3 = 0;
            int counter4 = 0;
            int counter5 = 0;
            int counter6 = 0;
            int counter7 = 0;
            int counter8 = 0;

            Action action1 = () =>
            {
                Interlocked.Increment(ref counter1);
            };
            Action action2 = () =>
            {
                Interlocked.Increment(ref counter2);
            };
            Action action3 = () =>
            {
                Interlocked.Increment(ref counter3);
            };
            Action action4 = () =>
            {
                Interlocked.Increment(ref counter4);
            };
            Action action5 = () =>
            {
                Interlocked.Increment(ref counter5);
            };
            Action action6 = () =>
            {
                Interlocked.Increment(ref counter6);
            };
            Action action7 = () =>
            {
                Interlocked.Increment(ref counter7);
            };
            Action action8 = () =>
            {
                Interlocked.Increment(ref counter8);
            };

            scheduler.OnWorker("worker1");
            scheduler.Schedule(action1).EveryMinute();

            scheduler.OnWorker("worker2");
            scheduler.Schedule(action2).EveryMinute();
            scheduler.Schedule(action2).EveryMinute();

            scheduler.OnWorker("worker3");
            scheduler.Schedule(action3).EveryMinute();
            scheduler.Schedule(action3).EveryMinute();
            scheduler.Schedule(action3).EveryMinute();

            scheduler.OnWorker("worker4");
            scheduler.Schedule(action4).EveryMinute();
            scheduler.Schedule(action4).EveryMinute();
            scheduler.Schedule(action4).EveryMinute();
            scheduler.Schedule(action4).EveryMinute();

            scheduler.OnWorker("worker5");
            scheduler.Schedule(action5).EveryMinute();
            scheduler.Schedule(action5).EveryMinute();
            scheduler.Schedule(action5).EveryMinute();
            scheduler.Schedule(action5).EveryMinute();
            scheduler.Schedule(action5).EveryMinute();

            scheduler.OnWorker("worker6");
            scheduler.Schedule(action6).EveryMinute();
            scheduler.Schedule(action6).EveryMinute();
            scheduler.Schedule(action6).EveryMinute();
            scheduler.Schedule(action6).EveryMinute();
            scheduler.Schedule(action6).EveryMinute();
            scheduler.Schedule(action6).EveryMinute();

            scheduler.OnWorker("worker7");
            scheduler.Schedule(action7).EveryMinute();
            scheduler.Schedule(action7).EveryMinute();
            scheduler.Schedule(action7).EveryMinute();
            scheduler.Schedule(action7).EveryMinute();
            scheduler.Schedule(action7).EveryMinute();
            scheduler.Schedule(action7).EveryMinute();
            scheduler.Schedule(action7).EveryMinute();

            scheduler.OnWorker("worker8");
            scheduler.Schedule(action8).EveryMinute();
            scheduler.Schedule(action8).EveryMinute();
            scheduler.Schedule(action8).EveryMinute();
            scheduler.Schedule(action8).EveryMinute();
            scheduler.Schedule(action8).EveryMinute();
            scheduler.Schedule(action8).EveryMinute();
            scheduler.Schedule(action8).EveryMinute();
            scheduler.Schedule(action8).EveryMinute();

            await Task.WhenAll(
                scheduler.RunAtAsync(DateTime.Parse("2018/06/07")),
                scheduler.RunAtAsync(DateTime.Parse("2018/06/07")),
                scheduler.RunAtAsync(DateTime.Parse("2018/06/07")),
                scheduler.RunAtAsync(DateTime.Parse("2018/06/07"))
            );

            // Everything ran ok?
            Assert.True(counter1 == 4);
            Assert.True(counter2 == 8);
            Assert.True(counter3 == 12);
            Assert.True(counter4 == 16);
            Assert.True(counter5 == 20);
            Assert.True(counter6 == 24);
            Assert.True(counter7 == 28);
            Assert.True(counter8 == 32);

        }

        [Fact]
        public async Task ScheduleWorkerSuccessfulWithDifferentSchedules()
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), null);

            int counter1 = 0;
            int counter2 = 0;
            int counter3 = 0;
            int counter4 = 0;

            Action action1 = () =>
            {
                counter1++;
            };
            Action action2 = () =>
            {
                counter2++;
            };
            Action action3 = () =>
            {
                counter3++;
            };
            Action action4 = () =>
            {
                counter4++;
            };

            scheduler.OnWorker("worker1");
            scheduler.Schedule(action1).EveryMinute();
            scheduler.Schedule(action1).Daily();
            scheduler.Schedule(action1).Weekly();

            scheduler.OnWorker("worker2");
            scheduler.Schedule(action2).EveryMinute();
            scheduler.Schedule(action2).EveryMinute();
            scheduler.Schedule(action2).Daily();
            scheduler.Schedule(action2).Weekly();

            scheduler.OnWorker("worker3");
            scheduler.Schedule(action3).EveryMinute();
            scheduler.Schedule(action3).EveryMinute();
            scheduler.Schedule(action3).EveryMinute();
            scheduler.Schedule(action3).Daily();
            scheduler.Schedule(action3).Weekly();

            scheduler.OnWorker("worker4");
            scheduler.Schedule(action4).EveryMinute();
            scheduler.Schedule(action4).EveryMinute();
            scheduler.Schedule(action4).EveryMinute();
            scheduler.Schedule(action4).EveryMinute();
            scheduler.Schedule(action4).Daily();
            scheduler.Schedule(action4).Weekly();

            await scheduler.RunAtAsync(DateTime.Parse("2018/06/07 12:05 am"));

            Assert.True(counter1 == 1);
            Assert.True(counter2 == 2);
            Assert.True(counter3 == 3);
            Assert.True(counter4 == 4);
        }
    }
}