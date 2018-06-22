# Task Scheduling

Usually, you have to configure a cron job or a task via Windows Task Scheduler to get a single or multiple re-occuring tasks to run. With coravel you can setup all your scheduled tasks in one place! And it's super easy to use!

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

A `Scheduler` is provided to you for configuring what tasks you want to schedule. It has a method `Schedule()` which accepts a `System.Action`. This contains the logic / code of the task you want to run.

After calling `Schedule()` you can chain method calls further to specify:

- The interval of when your task should be run (once a minute? every hour? etc.)
- Specific times when you want your task to run
- Restricting which days your task is allowed to run on (Monday's only? etc.)

Example: Run a task once an hour only on Mondays.

```c#
scheduler.Schedule(
    () => Console.WriteLine("Hourly on Mondays.")
)
.Hourly()
.Monday();
```

Example: Run a task every day at 1pm

```c#
scheduler.Schedule(
    () => Console.WriteLine("Daily at 1 pm.")
)
.DailyAtHour(13); // Or .DailyAt(13, 00)
```

## Error Handling

Any tasks that throw errors __will just be skipped__ and the next task in line will be invoked.

If you want to catch errors and do something specific with them you may use the `OnError()` extensions method on your `IServiceCollection`.

```c#
services.OnError((exception) => doSomethingWithException(exception));
```

You can, of course, add error handling inside your specific tasks too.

## Scheduling Tasks

After you have called the `Schedule()` method on the `Scheduler`, you can begin to configure the schedule constraints of your task.

### Intervals

First, methods to apply interval constraints are available.

#### Basic Intervals

These methods tell your task to execute at basic intervals.

Using any of these methods will cause the task to be executed immedately after your app has started. Then they will only be
executed again once the specific interval has been reached.

If you restart your app these methods will cause all tasks to run again on start. To avoid this, use an interval method with time constraints (see below).

- `EveryMinute();`
- `EveryFiveMinutes();`
- `EveryTenMinutes();`
- `EveryFifteenMinutes();`
- `EveryThirtyMinutes();`
- `Hourly();`
- `Daily();`
- `Weekly();`

#### Intervals With Time Contraints

These methods allow you specify an interval and a time constraint so that your scheduling is more specific and consistent.

Please note that the scheduler is using _UTC_ time. So, for example, using `DailyAt(13, 00)` will run your task daily at 1pm _UTC_ time.

- `HourlyAt(int minute)`
- `DailyAtHour(int hour)`
- `DailyAt(int hour, int minute)`

### Day Constraints

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