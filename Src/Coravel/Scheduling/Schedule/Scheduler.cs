using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Queuing;
using Coravel.Queuing.Interfaces;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Coravel.Scheduling.Schedule.Helpers;
using Coravel.Scheduling.Schedule.Event;
using Microsoft.Extensions.DependencyInjection;
using Coravel.Invocable;

namespace Coravel.Scheduling.Schedule
{
    public class Scheduler : IScheduler, ISchedulerConfiguration
    {
        private ConcurrentBag<ScheduledEvent> _tasks;
        private Action<Exception> _errorHandler;
        private ILogger<IScheduler> _logger;
        private IMutex _mutex;
        private readonly int EventLockTimeout_24Hours = 1440;
        private IServiceScopeFactory _scopeFactory;
        private bool _isRunning = false;

        public Scheduler(IMutex mutex, IServiceScopeFactory scopeFactory)
        {
            this._tasks = new ConcurrentBag<ScheduledEvent>();
            this._mutex = mutex;
            this._scopeFactory = scopeFactory;
        }

        public IScheduleInterval Schedule(Action actionToSchedule)
        {
            ScheduledEvent scheduled = new ScheduledEvent(actionToSchedule);
            this._tasks.Add(scheduled);
            return scheduled;
        }

        public IScheduleInterval ScheduleAsync(Func<Task> asyncTaskToSchedule)
        {
            ScheduledEvent scheduled = new ScheduledEvent(asyncTaskToSchedule);
            this._tasks.Add(scheduled);
            return scheduled;
        }

        public IScheduleInterval Schedule<T>() where T : IInvocable
        {
            ScheduledEvent scheduled = ScheduledEvent.WithInvocable<T>(this._scopeFactory);
            this._tasks.Add(scheduled);
            return scheduled;
        }

        public async Task RunSchedulerAsync()
        {
            this._isRunning = true;
            DateTime utcNow = DateTime.UtcNow;
            await this.RunAtAsync(utcNow);
            this._isRunning = false;
        }

        public async Task RunAtAsync(DateTime utcDate)
        {
            var activeTasks = new List<Task>();
            foreach (var scheduledEvent in this._tasks)
            {
                if (scheduledEvent.IsDue(utcDate))
                {
                    activeTasks.Add(InvokeEvent(scheduledEvent));
                }
            }

            await Task.WhenAll(activeTasks);
        }

        public ISchedulerConfiguration OnError(Action<Exception> onError)
        {
            this._errorHandler = onError;
            return this;
        }

        public ISchedulerConfiguration LogScheduledTaskProgress(ILogger<IScheduler> logger)
        {
            this._logger = logger;
            return this;
        }

        public bool IsStillRunning() => this._isRunning; // Will be read from another thread. There will only be one writer.

        private async Task InvokeEvent(ScheduledEvent scheduledEvent)
        {
            try
            {
                async Task Invoke()
                {
                    this._logger?.LogInformation("Scheduled task started...");
                    await scheduledEvent.InvokeScheduledEvent();
                    this._logger?.LogInformation("Scheduled task finished...");
                };

                Func<Task> invokeDelegate = Invoke;

                if (scheduledEvent.IsLongRunning())
                {
                    // Cpu heavy tasks / long running tasks will be run on a different thread.
                    // Any cpu heavy tasks will not prevent other tasks from being executed.
                    invokeDelegate = () => Task.Run(Invoke);
                }

                if (scheduledEvent.ShouldPreventOverlapping())
                {
                    if (this._mutex.TryGetLock(scheduledEvent.OverlappingUniqueIdentifier(), EventLockTimeout_24Hours))
                    {
                        try
                        {
                            await invokeDelegate();
                        }
                        finally
                        {
                            this._mutex.Release(scheduledEvent.OverlappingUniqueIdentifier());
                        }
                    }
                }
                else
                {
                    await invokeDelegate();
                }

            }
            catch (Exception e)
            {
                this._logger?.LogError("A scheduled task threw an Exception: " + e.Message);
                if (this._errorHandler != null)
                {
                    this._errorHandler(e);
                }
            }
        }
    }
}