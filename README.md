# Coravel

Inspired by all the awesome features that are baked into the Laravel PHP framework - coravel seeks to provide additional features that .Net Core lacks like:

- Easy Task Scheduling
- Simple Queuing
- Additional command line tools integrated with other coraval features

## Task Scheduling

### Initial Setup

In your .NET Core app's `Startup.cs` file, inside the `ConfigureServices()` method, add the following:

```c#
services.AddScheduler(scheduler =>
    {
        scheduler.Schedule(
            () => Console.WriteLine("Every minute. Ran at utc" + DateTime.UtcNow.ToLongTimeString())
        ).EveryMinute();
    }
);
```

The `AddScheduler()` method will configure a new Hosted Service that will run in the background while your app is running.

This method has an instance of a `Scheduler` provided to you for configuring what tasks you want to schedule. It has a method `Schedule()` which accepts a `System.Action`. This contains the logic / code of the task you want to run.

The output of the `Schedule()` method will return an object that implements the interface `IScheduled`. This provides methods for specifying the interval of when your tasks should be run (once a minute? every hour? etc.)

### Scheduling Tasks With Varying Intervals

Another example, with multiple scheduled tasks:

```c#

services.AddScheduler(scheduler =>
{
    scheduler.Schedule(
        () => Console.WriteLine("Every minute. Ran at utc " + DateTime.UtcNow)
    ).EveryMinute();

    scheduler.Schedule(
        () => Console.WriteLine("Every 10 minutes. Ran at utc " + DateTime.UtcNow)
    ).EveryTenMinutes();

    scheduler.Schedule(
        () => Console.WriteLine("Every hour. Ran at utc " + DateTime.UtcNow)
    ).Hourly();
});
```

### ISchedule Interval Methods

TBA

## Job Queing

TBA