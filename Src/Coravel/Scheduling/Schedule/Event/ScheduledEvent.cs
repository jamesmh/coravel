using System;
using System.Threading;
using System.Threading.Tasks;
using Coravel.Invocable;
using Coravel.Scheduling.Schedule.Cron;
using Coravel.Scheduling.Schedule.Interfaces;
using Coravel.Tasks;
using Coravel.Scheduling.Schedule.Zoned;
using Microsoft.Extensions.DependencyInjection;

namespace Coravel.Scheduling.Schedule.Event;

public sealed class ScheduledEvent : IScheduleInterval, IScheduledEventConfiguration
{
    private CronExpression? _expression;
    private readonly ActionOrAsyncFunc? _scheduledAction;
    private Type? _invocableType = null;
    private bool _preventOverlapping = false;
    private string _eventUniqueId;
    private readonly IServiceScopeFactory _scopeFactory;
    private Func<Task<bool>>? _whenPredicate;
    private bool _isScheduledPerSecond = false;
    private int? _secondsInterval = null;
    private object[]? _constructorParameters = null;
    private ZonedTime _zonedTime = ZonedTime.AsUTC();
    private bool _runOnceAtStart = false;
    private bool _runOnce = false;
    private bool _wasPreviouslyRun = false;

    public ScheduledEvent(Action scheduledAction, IServiceScopeFactory scopeFactory) : this(scopeFactory) => _scheduledAction = new ActionOrAsyncFunc(scheduledAction);

    public ScheduledEvent(Func<Task> scheduledAsyncTask, IServiceScopeFactory scopeFactory) : this(scopeFactory) => _scheduledAction = new ActionOrAsyncFunc(scheduledAsyncTask);

    private ScheduledEvent(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _eventUniqueId = Guid.NewGuid().ToString();
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
        var scheduledEvent = new ScheduledEvent(scopeFactory)
        {
            _invocableType = invocableType
        };

        return scheduledEvent;
    }

    private static readonly int _OneMinuteAsSeconds = 60;

    public bool IsDue(DateTime utcNow)
    {
        var zonedNow = _zonedTime.Convert(utcNow);

        if (_isScheduledPerSecond)
        {
            var isSecondDue = IsSecondsDue(zonedNow);
            var isWeekDayDue = _expression?.IsWeekDayDue(zonedNow) ?? false;
            return isSecondDue && isWeekDayDue;
        }
        else
        {
            return _expression?.IsDue(zonedNow) ?? false;
        }
    }

    public async Task InvokeScheduledEvent(CancellationToken cancellationToken)
    {
        if (await WhenPredicateFails())
        {
            return;
        }

        if (_invocableType is null && _scheduledAction != null)
        {
            await _scheduledAction.Invoke();
        }
        else
        {
            await using AsyncServiceScope scope = new(_scopeFactory.CreateAsyncScope());
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

    public bool ShouldPreventOverlapping() => _preventOverlapping;

    public string OverlappingUniqueIdentifier() => _eventUniqueId;

    public bool IsScheduledCronBasedTask() => !_isScheduledPerSecond;

    public IScheduledEventConfiguration Daily()
    {
        _expression = new CronExpression("00 00 * * *");
        return this;
    }

    public IScheduledEventConfiguration DailyAtHour(int hour)
    {
        _expression = new CronExpression($"00 {hour} * * *");
        return this;
    }

    public IScheduledEventConfiguration DailyAt(int hour, int minute)
    {
        _expression = new CronExpression($"{minute} {hour} * * *");
        return this;
    }

    public IScheduledEventConfiguration Hourly()
    {
        _expression = new CronExpression($"00 * * * *");
        return this;
    }

    public IScheduledEventConfiguration HourlyAt(int minute)
    {
        _expression = new CronExpression($"{minute} * * * *");
        return this;
    }

    public IScheduledEventConfiguration EveryMinute()
    {
        _expression = new CronExpression($"* * * * *");
        return this;
    }

    public IScheduledEventConfiguration EveryFiveMinutes()
    {
        _expression = new CronExpression($"*/5 * * * *");
        return this;
    }

    public IScheduledEventConfiguration EveryTenMinutes()
    {
        // todo fix "*/10" in cron part
        _expression = new CronExpression($"*/10 * * * *");
        return this;
    }

    public IScheduledEventConfiguration EveryFifteenMinutes()
    {
        _expression = new CronExpression($"*/15 * * * *");
        return this;
    }

    public IScheduledEventConfiguration EveryThirtyMinutes()
    {
        _expression = new CronExpression($"*/30 * * * *");
        return this;
    }

    public IScheduledEventConfiguration Weekly()
    {
        _expression = new CronExpression($"00 00 * * 1");
        return this;
    }

    public IScheduledEventConfiguration Monthly()
    {
        _expression = new CronExpression($"00 00 1 * *");
        return this;
    }

    public IScheduledEventConfiguration Cron(string cronExpression)
    {
        _expression = new CronExpression(cronExpression);
        return this;
    }

    public IScheduledEventConfiguration Monday()
    {
        _expression?.AppendWeekDay(DayOfWeek.Monday);
        return this;
    }

    public IScheduledEventConfiguration Tuesday()
    {
        _expression?.AppendWeekDay(DayOfWeek.Tuesday);
        return this;
    }

    public IScheduledEventConfiguration Wednesday()
    {
        _expression?.AppendWeekDay(DayOfWeek.Wednesday);
        return this;
    }

    public IScheduledEventConfiguration Thursday()
    {
        _expression?.AppendWeekDay(DayOfWeek.Thursday);
        return this;
    }

    public IScheduledEventConfiguration Friday()
    {
        _expression?.AppendWeekDay(DayOfWeek.Friday);
        return this;
    }

    public IScheduledEventConfiguration Saturday()
    {
        _expression?.AppendWeekDay(DayOfWeek.Saturday);
        return this;
    }

    public IScheduledEventConfiguration Sunday()
    {
        _expression?.AppendWeekDay(DayOfWeek.Sunday);
        return this;
    }

    public IScheduledEventConfiguration Weekday()
    {
        Monday()
            .Tuesday()
            .Wednesday()
            .Thursday()
            .Friday();
        return this;
    }

    public IScheduledEventConfiguration Weekend()
    {
        Saturday()
            .Sunday();
        return this;
    }

    public IScheduledEventConfiguration PreventOverlapping(string uniqueIdentifier)
    {
        _preventOverlapping = true;
        return AssignUniqueIndentifier(uniqueIdentifier);
    }

    public IScheduledEventConfiguration When(Func<Task<bool>> predicate)
    {
        _whenPredicate = predicate;
        return this;
    }

    public IScheduledEventConfiguration AssignUniqueIndentifier(string uniqueIdentifier)
    {
        _eventUniqueId = uniqueIdentifier;
        return this;
    }

    public Type? InvocableType() => _invocableType;

    private async Task<bool> WhenPredicateFails()
    {
        return _whenPredicate != null && (!await _whenPredicate.Invoke());
    }

    public IScheduledEventConfiguration EverySecond()
    {
        _secondsInterval = 1;
        _isScheduledPerSecond = true;
        _expression = new CronExpression("* * * * *");
        return this;
    }

    public IScheduledEventConfiguration EveryFiveSeconds()
    {
        _secondsInterval = 5;
        _isScheduledPerSecond = true;
        _expression = new CronExpression("* * * * *");
        return this;
    }

    public IScheduledEventConfiguration EveryTenSeconds()
    {
        _secondsInterval = 10;
        _isScheduledPerSecond = true;
        _expression = new CronExpression("* * * * *");
        return this;
    }

    public IScheduledEventConfiguration EveryFifteenSeconds()
    {
        _secondsInterval = 15;
        _isScheduledPerSecond = true;
        _expression = new CronExpression("* * * * *");
        return this;
    }

    public IScheduledEventConfiguration EveryThirtySeconds()
    {
        _secondsInterval = 30;
        _isScheduledPerSecond = true;
        _expression = new CronExpression("* * * * *");
        return this;
    }

    public IScheduledEventConfiguration EverySeconds(int seconds)
    {
        if (seconds < 1 || seconds > 59)
        {
            throw new ArgumentException(
                "When calling 'EverySeconds(int seconds)', 'seconds' must be between 0 and 60");
        }

        _secondsInterval = seconds;
        _isScheduledPerSecond = true;
        _expression = new CronExpression("* * * * *");
        return this;
    }

    public IScheduledEventConfiguration Zoned(TimeZoneInfo timeZoneInfo)
    {
        _zonedTime = new ZonedTime(timeZoneInfo);
        return this;
    }

    public IScheduledEventConfiguration RunOnceAtStart()
    {
        _runOnceAtStart = true;
        return this;
    }

    public IScheduledEventConfiguration Once()
    {
        _runOnce = true;
        return this;
    }

    private bool IsSecondsDue(DateTime utcNow)
    {
        if (utcNow.Second == 0)
        {
            return _OneMinuteAsSeconds % _secondsInterval == 0;
        }
        else
        {
            return utcNow.Second % _secondsInterval == 0;
        }
    }

    internal bool ShouldRunOnceAtStart() => _runOnceAtStart;

    private object GetInvocable(IServiceProvider serviceProvider)
    {
        if (_invocableType == null)
        {
            throw new ScheduledEventGetInvocableException($"{nameof(_invocableType)} could not be null");
        }

        if (_constructorParameters?.Length > 0)
        {
            return ActivatorUtilities.CreateInstance(serviceProvider, _invocableType,
                _constructorParameters);
        }

        return serviceProvider.GetRequiredService(_invocableType);
    }

    private bool PreviouslyRanAndMarkedToRunOnlyOnce() => _runOnce && _wasPreviouslyRun;

    private void MarkedAsExecutedOnce()
    {
        _wasPreviouslyRun = true;
    }

    private void UnScheduleIfWarranted()
    {
        if (PreviouslyRanAndMarkedToRunOnlyOnce())
        {
            using var scope = _scopeFactory.CreateScope();
            var scheduler = scope.ServiceProvider.GetService<IScheduler>() as Scheduler;
            scheduler?.TryUnschedule(_eventUniqueId);
        }
    }
}
