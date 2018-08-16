# Task Scheduling

Usually, you have to configure a cron job or a task via Windows Task Scheduler to get a single or multiple re-occurring tasks to run. With Coravel you can setup all your scheduled tasks in one place! And it's super easy to use!

## Initial Setup

In your .NET Core app's `Startup.cs` file, inside the `ConfigureServices()` method, add the following:

```c#
services.AddScheduler(scheduler =>
    {
        scheduler.Schedule(
            () => Console.WriteLine("Every minute during the week.")
        )
        .EveryMinute();
        .Weekday();
    }
);
```

This will run the task (which prints to the console) every minute and only on weekdays (not Sat or Sun). Simple enough?

## How Does It Work?

The `AddScheduler()` method will configure a new Hosted Service that will run in the background while your app is running.

A `Scheduler` is provided to you for configuring what tasks you want to schedule. You may use the `Schedule()` and `ScheduleAsync()` methods to schedule a task.

After calling `Schedule()` you can chain method calls further to specify:

- The interval of when your task should be run (once a minute? every hour? etc.)
- Specific times when you want your task to run
- Restricting which days your task is allowed to run on (Monday's only? etc.)

### Example: Run a task once an hour only on Mondays.

```c#
scheduler.Schedule(
    () => Console.WriteLine("Hourly on Mondays.")
)
.Hourly()
.Monday();
```

### Example: Run a task every day at 1pm

```c#
scheduler.Schedule(
    () => Console.WriteLine("Daily at 1 pm.")
)
.DailyAtHour(13); // Or .DailyAt(13, 00)
```

> Under the covers, all scheduling is similar to cron. Using `EveryFiveMinutes()`, for example, will trigger only when the minute is 0, 5, 10, 15, etc.

## Scheduling Tasks

After you have called the `Schedule()` method on the `Scheduler`, you can begin to configure the schedule constraints of your task.

```c#
scheduler.Schedule(
    () => Console.WriteLine("Scheduled task.")
)
.EveryMinute();
```

## Scheduling Async Tasks

Coravel will also handle scheduling async methods by using the `ScheduleAsync()` method. Note that this doesn't need to be awaited - the method or Func you provide _itself_ must be awaitable (as it will be invoked by the scheduler at a later time).

```c#
scheduler.ScheduleAsync(async () =>
{
    await Task.Delay(500);
    Console.WriteLine("async task");
})
.EveryMinute();
```

> You are able to register an async method when using `Schedule()` by mistake. Always use `ScheduleAsync()` when registering an async method.

## Scheduling Tasks Dynamically

You can add new scheduled tasks by using the `IScheduler` interface. Just inject the interface wherever DI is available.

## Intervals

First, methods to apply interval constraints are available.

### Basic Intervals

These methods tell your task to execute at basic intervals.

Using any of these methods will cause the task to be executed immediately after your app has started. Then they will only be executed again once the specific interval has been reached.

If you restart your app these methods will cause all tasks to run again on start. To avoid this, use an interval method with time constraints (see below).

- `EveryMinute();`
- `EveryFiveMinutes();`
- `EveryTenMinutes();`
- `EveryFifteenMinutes();`
- `EveryThirtyMinutes();`
- `Hourly();`
- `Daily();`
- `Weekly();`

### Intervals With Time Constraints

These methods allow you specify an interval and a time constraint so that your scheduling is more specific and consistent.

_Please note that the scheduler is using UTC time. So, for example, using `DailyAt(13, 00)` will run your task daily at 1pm UTC time._

- `HourlyAt(int minute)`
- `DailyAtHour(int hour)`
- `DailyAt(int hour, int minute)`

### Cron Expressions

You can use the `Cron()` method to supply a cron like expression.

```c#
scheduler.Schedule(
    () => Console.WriteLine("Scheduled task.")
)
.Cron("00 00 1 * *"); // First day of the month at midnight.
```

Supported expressions are

- "\*" matches every value ("\* \* \* \* \*" - run every minute)
- "5" supply the actual value ("00 13 \* \* \*" - run at 1:00 pm daily)
- "5,6,7" matches each value ("00 1,2,3 \* \* \*" - run at 1:00 pm, 2:00 pm and 3:00 pm daily)
- "5-7" indicates a range of values ("00 1-3 \* \* \*" - same as above)
- "\*/5" indicates any value divisible by the value ("00 \*/2 \* \* \*" - run every two hours on the hour)

## Day Constraints

After specifying an interval, you can further chain to restrict what day(s) the scheduled task is allowed to run on.

All these methods are further chainable - like `Monday().Wednesday()`. This would mean only running the task on Mondays and Wednesdays. Be careful since you could do something like this `.Weekend().Weekday()` which basically means there are no constraints (it runs on any day).

- `Monday()`
- `Tuesday()`
- `Wednesday()`
- `Thursday()`
- `Friday()`
- `Saturday()`
- `Sunday()`
- `Weekday()`
- `Weekend()`

## Global Error Handling

Any tasks that throw errors **will just be skipped** and the next task in line will be invoked.

If you want to catch errors and do something specific with them you may use the `OnError()` method.

```c#
services.AddScheduler(scheduler =>
    // Assign your schedules
)
.OnError((exception) =>
    doSomethingWithException(exception)
);
```

You can, of course, add error handling inside your specific tasks too.

## Logging Executed Task Progress

Coravel uses the `ILogger` .Net Core interface to allow logging scheduled task progress.

In your `Startup.cs` file, you need to inject an instance of `IServiceProvider` to the constructor and assign it to a member field / property:

```c#
public Startup(IConfiguration configuration, /* Add this */ IServiceProvider services)
{
    Configuration = configuration;
    Services = services;
}

public IConfiguration Configuration { get; }

/* Add this property */
public IServiceProvider Services { get; }
```

Next, do the following:

```c#
services.AddScheduler(scheduler =>
{
    // Assign scheduled tasks...
})
.LogScheduledTaskProgress(Services.GetService<ILogger<IScheduler>>());
```

The `LogScheduledTaskProgress()` method accepts an instance of `ILogger<IScheduler>`, which is available through the service provider.
