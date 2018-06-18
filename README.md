# Coravel

Inspired by all the awesome features that are baked into the Laravel PHP framework - coravel seeks to provide additional features that .Net Core lacks like:

- Task Scheduling
- Queuing
- Command line tools integrated with coraval features
- More?

## Feature: Task Scheduling

Usually, you have to configure a cron job or a task via Windows Task Scheduler to get a single re-occuring task to work. With coravel you can setup all your scheduled tasks in one place! And it's super easy to use!

### Initial Setup

In your .NET Core app's `Startup.cs` file, inside the `ConfigureServices()` method, add the following:

```c#
services.AddScheduler(scheduler =>
    {
        scheduler.Schedule(
            () => Console.WriteLine("Every minute. Ran at utc" + DateTime.UtcNow.ToLongTimeString())
        )
        .EveryMinute();
        .Weekday();
    }
);
```

The `AddScheduler()` method will configure a new Hosted Service that will run in the background while your app is running.

A `Scheduler` is provided to you for configuring what tasks you want to schedule. It has a method `Schedule()` which accepts a `System.Action`. This contains the logic / code of the task you want to run.

After calling `Schedule()` you can chain method calls further to specify the interval of when your tasks should be run (once a minute? every hour? etc.) and further restrictions (Monday's only? etc.)

Example: Run a task once an hour only on Mondays.

```c#
scheduler.Schedule(
    () => Console.WriteLine("Every minute on Mondays only.")
)
.Hourly()
.Monday();
```

### Scheduling Methods

So far, these methods are available for specifying what the interval of your task's schedule can be:

- `AfterMinutes(int minutes);`
- `EveryMinute();`
- `EveryFiveMinutes();`
- `EveryTenMinutes();`
- `EveryFifteenMinutes();`
- `EveryThirtyMinutes();`
- `Hourly();`
- `Daily();`
- `Weekly();`

Further restrictions that can be chained:

- `Monday()`
- `Tuesday()`
- `Wednesday()`
- `Thursday()`
- `Friday()`
- `Saturday()`
- `Sunday()`
- `Weekday()`
- `Weekend()`

These may be called multiple times - like `Monday().Wednesday()`. This would mean only running the task on Mondays and Wednesdays.

## Feature: Task Queing

TBA
