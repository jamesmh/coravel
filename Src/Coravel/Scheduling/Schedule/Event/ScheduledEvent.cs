using System;
using System.Threading;
using System.Threading.Tasks;
using Coravel.Invocable;
using Coravel.Scheduling.Schedule.Cron;
using Coravel.Scheduling.Schedule.Interfaces;
using Coravel.Tasks;
using Coravel.Scheduling.Schedule.Zoned;
using Microsoft.Extensions.DependencyInjection;

namespace Coravel.Scheduling.Schedule.Event
{
    public class ScheduledEvent : IScheduleInterval, IScheduledEventConfiguration, IGetAllScheduleInfo
    {
        private CronExpression _expression;
        private ActionOrAsyncFunc _scheduledAction;
        private Type _invocableType = null;
        private bool _preventOverlapping = false;
        private string _eventUniqueId = null;
        private IServiceScopeFactory _scopeFactory;
        private Func<Task<bool>> _whenPredicate;
        private bool _isScheduledPerSecond = false;
        private int? _secondsInterval = null;
        private object[] _constructorParameters = null;
        private ZonedTime _zonedTime = ZonedTime.AsUTC();
        private bool _runOnceAtStart = false;
        private bool _runOnce = false;
        private bool _wasPreviouslyRun = false;

        public ScheduledEvent(Action scheduledAction, IServiceScopeFactory scopeFactory) : this(scopeFactory)
        {
            this._scheduledAction = new ActionOrAsyncFunc(scheduledAction);
        }

        public ScheduledEvent(Func<Task> scheduledAsyncTask, IServiceScopeFactory scopeFactory) : this(scopeFactory)
        {
            this._scheduledAction = new ActionOrAsyncFunc(scheduledAsyncTask);
        }

        private ScheduledEvent(IServiceScopeFactory scopeFactory)
        {
            this._scopeFactory = scopeFactory;
            this._eventUniqueId = Guid.NewGuid().ToString();
        }

        public static ScheduledEvent WithInvocable<T>(IServiceScopeFactory scopeFactory) where T : IInvocable
        {
            return WithInvocableType(typeof(T), scopeFactory);
        }

        internal static ScheduledEvent WithInvocableAndParams<T>(IServiceScopeFactory scopeFactory, object[] parameters)
            where T : IInvocable
        {
            var scheduledEvent = WithInvocableType(typeof(T), scopeFactory);
            scheduledEvent._constructorParameters = parameters;
            return scheduledEvent;
        }

        internal static ScheduledEvent WithInvocableAndParams(Type invocableType, IServiceScopeFactory scopeFactory, object[] parameters)
        {
            if (!typeof(IInvocable).IsAssignableFrom(invocableType))
            {
                throw new ArgumentException(
                    $"When using {nameof(IScheduler.ScheduleWithParams)}() you must supply a type that inherits from {nameof(IInvocable)}.",
                    nameof(invocableType));
            }

            var scheduledEvent = WithInvocableType(invocableType, scopeFactory);
            scheduledEvent._constructorParameters = parameters;
            return scheduledEvent;
        }

        public static ScheduledEvent WithInvocableType(Type invocableType, IServiceScopeFactory scopeFactory)
        {
            var scheduledEvent = new ScheduledEvent(scopeFactory);
            scheduledEvent._invocableType = invocableType;
            return scheduledEvent;
        }

        private static readonly int _OneMinuteAsSeconds = 60;

        public bool IsDue(DateTime utcNow)
        {
            var zonedNow = this._zonedTime.Convert(utcNow);

            if (this._isScheduledPerSecond)
            {
                var isSecondDue = this.IsSecondsDue(zonedNow);
                var isWeekDayDue = this._expression.IsWeekDayDue(zonedNow);
                return isSecondDue && isWeekDayDue;
            }
            else
            {
                return this._expression.IsDue(zonedNow);
            }
        }

        public async Task InvokeScheduledEvent(CancellationToken cancellationToken)
        {
            if (await WhenPredicateFails())
            {
                return;
            }

            if (this._invocableType is null)
            {
                await this._scheduledAction.Invoke();
            }
            else
            {
                await using AsyncServiceScope scope = new(this._scopeFactory.CreateAsyncScope());
                if (GetInvocable(scope.ServiceProvider) is IInvocable invocable)
                {
                    if (invocable is ICancellableInvocable cancellableInvokable)
                    {
                        cancellableInvokable.CancellationToken = cancellationToken;
                    }

                    await invocable.Invoke();
                }
            }

            MarkedAsExecutedOnce();
            UnScheduleIfWarranted();
        }

        public bool ShouldPreventOverlapping() => this._preventOverlapping;

        public string OverlappingUniqueIdentifier() => this._eventUniqueId;

        public bool IsScheduledCronBasedTask() => !this._isScheduledPerSecond;

        public ScheduleInfo GetScheduleInfo()
        {
            return new ScheduleInfo(
                cronExpression: this._expression?.ToString() ?? string.Empty,
                isScheduledPerSecond: this._isScheduledPerSecond,
                secondsInterval: this._secondsInterval,
                invocableType: this._invocableType,
                preventOverlapping: this._preventOverlapping,
                eventUniqueId: this._eventUniqueId,
                hasWhenPredicates: this._whenPredicate != null,
                zonedTimeZone: this._zonedTime?.TimeZoneInfo ?? TimeZoneInfo.Utc,
                runOnceAtStart: this._runOnceAtStart,
                runOnce: this._runOnce
            );
        }

        public IScheduledEventConfiguration Daily()
        {
            this._expression = new CronExpression("00 00 * * *");
            return this;
        }

        public IScheduledEventConfiguration DailyAtHour(int hour)
        {
            this._expression = new CronExpression($"00 {hour} * * *");
            return this;
        }

        public IScheduledEventConfiguration DailyAt(int hour, int minute)
        {
            this._expression = new CronExpression($"{minute} {hour} * * *");
            return this;
        }

        public IScheduledEventConfiguration Hourly()
        {
            this._expression = new CronExpression($"00 * * * *");
            return this;
        }

        public IScheduledEventConfiguration HourlyAt(int minute)
        {
            this._expression = new CronExpression($"{minute} * * * *");
            return this;
        }

        public IScheduledEventConfiguration EveryMinute()
        {
            this._expression = new CronExpression($"* * * * *");
            return this;
        }

        public IScheduledEventConfiguration EveryFiveMinutes()
        {
            this._expression = new CronExpression($"*/5 * * * *");
            return this;
        }

        public IScheduledEventConfiguration EveryTenMinutes()
        {
            // todo fix "*/10" in cron part
            this._expression = new CronExpression($"*/10 * * * *");
            return this;
        }

        public IScheduledEventConfiguration EveryFifteenMinutes()
        {
            this._expression = new CronExpression($"*/15 * * * *");
            return this;
        }

        public IScheduledEventConfiguration EveryThirtyMinutes()
        {
            this._expression = new CronExpression($"*/30 * * * *");
            return this;
        }

        public IScheduledEventConfiguration Weekly()
        {
            this._expression = new CronExpression($"00 00 * * 1");
            return this;
        }

        public IScheduledEventConfiguration Monthly()
        {
            this._expression = new CronExpression($"00 00 1 * *");
            return this;
        }

        public IScheduledEventConfiguration Cron(string cronExpression)
        {
            this._expression = new CronExpression(cronExpression);
            return this;
        }

        public IScheduledEventConfiguration Monday()
        {
            this._expression.AppendWeekDay(DayOfWeek.Monday);
            return this;
        }

        public IScheduledEventConfiguration Tuesday()
        {
            this._expression.AppendWeekDay(DayOfWeek.Tuesday);
            return this;
        }

        public IScheduledEventConfiguration Wednesday()
        {
            this._expression.AppendWeekDay(DayOfWeek.Wednesday);
            return this;
        }

        public IScheduledEventConfiguration Thursday()
        {
            this._expression.AppendWeekDay(DayOfWeek.Thursday);
            return this;
        }

        public IScheduledEventConfiguration Friday()
        {
            this._expression.AppendWeekDay(DayOfWeek.Friday);
            return this;
        }

        public IScheduledEventConfiguration Saturday()
        {
            this._expression.AppendWeekDay(DayOfWeek.Saturday);
            return this;
        }

        public IScheduledEventConfiguration Sunday()
        {
            this._expression.AppendWeekDay(DayOfWeek.Sunday);
            return this;
        }

        public IScheduledEventConfiguration Weekday()
        {
            this.Monday()
                .Tuesday()
                .Wednesday()
                .Thursday()
                .Friday();
            return this;
        }

        public IScheduledEventConfiguration Weekend()
        {
            this.Saturday()
                .Sunday();
            return this;
        }

        public IScheduledEventConfiguration PreventOverlapping(string uniqueIdentifier)
        {
            this._preventOverlapping = true;
            return this.AssignUniqueIndentifier(uniqueIdentifier);
        }

        public IScheduledEventConfiguration When(Func<Task<bool>> predicate)
        {
            this._whenPredicate = predicate;
            return this;
        }

        public IScheduledEventConfiguration AssignUniqueIndentifier(string uniqueIdentifier)
        {
            this._eventUniqueId = uniqueIdentifier;
            return this;
        }

        public Type InvocableType() => this._invocableType;

        private async Task<bool> WhenPredicateFails()
        {
            return this._whenPredicate != null && (!await _whenPredicate.Invoke());
        }

        public IScheduledEventConfiguration EverySecond()
        {
            this._secondsInterval = 1;
            this._isScheduledPerSecond = true;
            this._expression = new CronExpression("* * * * *");
            return this;
        }

        public IScheduledEventConfiguration EveryFiveSeconds()
        {
            this._secondsInterval = 5;
            this._isScheduledPerSecond = true;
            this._expression = new CronExpression("* * * * *");
            return this;
        }

        public IScheduledEventConfiguration EveryTenSeconds()
        {
            this._secondsInterval = 10;
            this._isScheduledPerSecond = true;
            this._expression = new CronExpression("* * * * *");
            return this;
        }

        public IScheduledEventConfiguration EveryFifteenSeconds()
        {
            this._secondsInterval = 15;
            this._isScheduledPerSecond = true;
            this._expression = new CronExpression("* * * * *");
            return this;
        }

        public IScheduledEventConfiguration EveryThirtySeconds()
        {
            this._secondsInterval = 30;
            this._isScheduledPerSecond = true;
            this._expression = new CronExpression("* * * * *");
            return this;
        }

        public IScheduledEventConfiguration EverySeconds(int seconds)
        {
            if (seconds < 1 || seconds > 59)
            {
                throw new ArgumentException(
                    "When calling 'EverySeconds(int seconds)', 'seconds' must be between 0 and 60");
            }

            this._secondsInterval = seconds;
            this._isScheduledPerSecond = true;
            this._expression = new CronExpression("* * * * *");
            return this;
        }

        public IScheduledEventConfiguration Zoned(TimeZoneInfo timeZoneInfo)
        {
            this._zonedTime = new ZonedTime(timeZoneInfo);
            return this;
        }

        public IScheduledEventConfiguration RunOnceAtStart()
        {
            this._runOnceAtStart = true;
            return this;
        }
        
        public IScheduledEventConfiguration Once()
        {
            this._runOnce = true;
            return this;
        }
        
        private bool IsSecondsDue(DateTime utcNow)
        {
            if (utcNow.Second == 0)
            {
                return _OneMinuteAsSeconds % this._secondsInterval == 0;
            }
            else
            {
                return utcNow.Second % this._secondsInterval == 0;
            }
        }

        internal bool ShouldRunOnceAtStart() => this._runOnceAtStart;
        
        private object GetInvocable(IServiceProvider serviceProvider)
        {
            if (this._constructorParameters?.Length > 0)
            {
                return ActivatorUtilities.CreateInstance(serviceProvider, this._invocableType,
                    this._constructorParameters);
            }

            return serviceProvider.GetRequiredService(this._invocableType);
        }

        private bool PreviouslyRanAndMarkedToRunOnlyOnce() => this._runOnce && this._wasPreviouslyRun;
        
        private void MarkedAsExecutedOnce()
        {
            this._wasPreviouslyRun = true;
        }
        
        private void UnScheduleIfWarranted()
        {
            if (PreviouslyRanAndMarkedToRunOnlyOnce())
            {
                using var scope = this._scopeFactory.CreateScope();
                var scheduler = scope.ServiceProvider.GetService<IScheduler>() as Scheduler;
                scheduler.TryUnschedule(this._eventUniqueId);
            }
        }
    }
}
