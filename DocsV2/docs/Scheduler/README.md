---
meta:
  - name: description
    content: Coravel is a near-zero config .NET Core library that makes Task Scheduling, Caching, Queuing, Mailing, Event Broadcasting (and more) a breeze!
  - name: keywords
    content: dotnet dotnetcore ".NET Core task scheduling" ".NET Core scheduler" ".NET Core framework" ".NET Core Queue" ".NET Core Queuing" ".NET Core Caching" Coravel
---

# Task Scheduling

[[toc]]

Usually, you have to configure a cron job or a task via Windows Task Scheduler to get a single or multiple re-occurring tasks to run. 

With Coravel you can setup all your scheduled tasks in one place using a simple, elegant, fluent syntax - in code!

Scheduling is now a breeze!

## Config 

In your .NET Core app's `Startup.cs` file, inside the `ConfigureServices()` method, add the following:

```csharp
services.AddScheduler()
```

Then in the `Configure()` method, you can use the scheduler:

```csharp
var provider = app.ApplicationServices;
provider.UseScheduler(scheduler =>
{
    scheduler.Schedule(
        () => Console.WriteLine("Every minute during the week.")
    )
    .EveryMinute()
    .Weekday();
});
```

Simple enough?

## Scheduling Tasks

### Invocables

:::tip
Using invocables is the recommended way to schedule your tasks.

To learn about creating invocables [see the docs.](/Invocables/)
:::

In essence, to schedule an invocable you must:

1. Ensure that your invocable is registered with the service provider as a scoped or transient service.

2. Use the `Schedule` method:

```csharp
scheduler
    .Schedule<GrabDataFromApiAndPutInDBInvocable>()
    .EveryTenMinutes();
```

What a simple, terse and expressive syntax! Easy Peasy!

P.s. If needed, you may schedule an invocable using reflection:

```csharp
scheduler.Schedule(typeof(GrabDataFromApiAndPutInDBInvocable))
```

### Async Tasks

Coravel will also handle scheduling `async` methods by using the `ScheduleAsync()` method.

:::tip
`ScheduleAsync` does not have to be awaited. The method or `Func` you provide _itself_ must be awaitable (e.g. returns a `Task` or `Task<T>`).
:::

```csharp
scheduler.ScheduleAsync(async () =>
{
    await Task.Delay(500);
    Console.WriteLine("async task");
})
.EveryMinute();
```

:::warning
You are able to register an async method when using `Schedule()` by mistake. Always use `ScheduleAsync()` when registering an async method.
:::

### Synchronous Tasks

While generally not recommended, there may be times when you aren't doing any async operations.

In this case, use `Schedule()`.

```csharp
scheduler.Schedule(
    () => Console.WriteLine("Scheduled task.")
)
.EveryMinute();
```

### Scheduling With Additional Parameters

The `ScheduleWithParams<T>()` method allows you to schedule the same invocable multiple times with different parameters.

```c#
private class BackupDatabaseTableInvocable : IInvocable
{
    private DbContext _dbContext;
    private string _tableName;

    public BackupDatabaseTableInvocable(DbContext dbContext, string tableName)
    {
        this._dbContext = dbContext; // Injected via DI.
        this._tableName = tableName; // injected via schedule configuration (see next code block).
    }

    public Task Invoke()
    {
        // Do the logic.
    }
}
```

You might configure it like this:

```c#
// In this case, backing up products 
// more often than users is required.

scheduler
    .ScheduleWithParams<BackupDatabaseTableInvocable>("[dbo].[Users]")
    .Daily();

scheduler
    .ScheduleWithParams<BackupDatabaseTableInvocable>("[dbo].[Products]")
    .EveryHour();
```

:::warning
Ensure that any parameters to be injected via dependency injection are listed first in your constructor arguments.
:::

## Intervals

After calling `Schedule` or `ScheduleAsync`, methods to specify the schedule interval are available.

| Method | Description |
|--------|--------------|
| `EverySecond()`                   | Run the task every second |
| `EveryFiveSeconds()`              | Run the task every five seconds |
| `EveryTenSeconds()`               | Run the task every ten seconds |
| `EveryFifteenSeconds()`           | Run the task every fifteen seconds |
| `EveryThirtySeconds()`            | Run the task every thirty seconds |
| `EverySeconds(3)`                 | Run the task every 3 seconds. |
| `EveryMinute()`                   | Run the task once a minute |
| `EveryFiveMinutes()`              | Run the task every five minutes |
| `EveryTenMinutes()`               | Run the task every ten minutes |
| `EveryFifteenMinutes()`           | Run the task every fifteen minutes |
| `EveryThirtyMinutes()`            | Run the task every thirty minutes |
| `Hourly()`                        | Run the task every hour |
| `HourlyAt(12)`                    | Run the task at 12 minutes past every hour |
| `Daily()`                         | Run the task once a day at midnight |
| `DailyAtHour(13)`                 | Run the task once a day at 1 p.m. UTC |
| `DailyAt(13, 30)`                 | Run the task once a day at 1:30 p.m. UTC |
| `Weekly()`                        | Run the task once a week |
| `Cron("* * * * *")`               | Run the task using a Cron expression |

:::tip
Please note that the scheduler is using UTC time.
:::

### Cron Expressions

Supported types of Cron expressions are:

- `* * * * *` run every minute
- `00 13 * * *` run at 1:00 pm daily
- `00 1,2,3 * * *` run at 1:00 pm, 2:00 pm and 3:00 pm daily
- `00 1-3 * * *` same as above
- `00 */2 * * *` run every two hours on the hour

## Day Constraints

After specifying an interval, you can further chain to restrict what day(s) the scheduled task is allowed to run on.

- `Monday()`
- `Tuesday()`
- `Wednesday()`
- `Thursday()`
- `Friday()`
- `Saturday()`
- `Sunday()`
- `Weekday()`
- `Weekend()`

All these methods are further chainable - like `Monday().Wednesday()`. This would mean only running the task on Mondays and Wednesdays. 

:::warning
Be careful since you could do something like `.Weekend().Weekday()`, which means there are no constraints (it runs on any day).
:::

## Extras

### Custom Boolean Constraint

Using the `When` method you can add additional restrictions to determine when your scheduled task should be executed.

```csharp
scheduler
    .Schedule(() => DoSomeStuff())
    .EveryMinute()
    .When(SomeMethodThatChecksStuff);
```

If you require access to dependencies that are registered with the service provider, it is recommended that you [schedule your tasks by using an invocable](#scheduling-invocables) and perform any further restriction logic there.

### Global Error Handling

Any tasks that throw errors **will just be skipped** and the next task in line will be invoked.

If you want to catch errors and do something specific with them you may use the `OnError()` method.

```csharp
provider.UseScheduler(scheduler =>
    // Assign your schedules
)
.OnError((exception) =>
    DoSomethingWithException(exception)
);
```

You can, of course, add error handling inside your specific tasks too.

### Logging Executed Task Progress

Coravel uses the `ILogger` .Net Core interface to allow logging scheduled task progress.

In your `Startup.cs` file, you need to inject an instance of `IServiceProvider` to the constructor and assign it to a member field / property:

```csharp
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

```csharp
provider.UseScheduler(scheduler =>
{
    // Assign scheduled tasks...
})
.LogScheduledTaskProgress(Services.GetService<ILogger<IScheduler>>());
```

The `LogScheduledTaskProgress()` method accepts an instance of `ILogger<IScheduler>`, which is available through the service provider.

## Prevent Overlapping Tasks

Sometimes you may have longer running tasks or tasks who's running time is variable. The normal behavior of the scheduler is to simply fire off a task if it is due.

But, what if the previous instance of this scheduled task is **still** running?

In this case, use the `PreventOverlapping` method to make sure there is only 1 running instance of your scheduled task. 

In other words, if the same scheduled task is due but another instance of it is still running, Coravel will just ignore the currently due task.

```csharp
scheduler
    .Schedule<SomeInvocable>()
    .EveryMinute()
    .PreventOverlapping("SomeInvocable");
```
This method takes in one parameter - a unique key (`string`) among all your scheduled tasks. This makes sure Coravel knows which task to lock and release.

## Schedule Workers

In order to make Coravel work well in web scenarios, the scheduler will run all due tasks sequentially (although asynchronously).

If you have longer running tasks - especially tasks that do some CPU intensive work - this will cause any subsequent tasks to execute much later than you might have expected or desired.

### What's A Worker?

Schedule workers solve this problem by allowing you to schedule groups of tasks that run in parallel! 

In other words, a schedule worker is just a pipeline that you can assign to a group of tasks which will have a dedicated thread.

### Usage

To begin assigning a schedule worker to a group of scheduled tasks use:

`OnWorker(string workerName)`

For example:

```csharp
scheduler.OnWorker("EmailTasks");
scheduler
    .Schedule<SendNightlyReportsEmailJob>().Daily();
scheduler
    .Schedule<SendPendingNotifications>().EveryMinute();

scheduler.OnWorker("CPUIntensiveTasks");
scheduler
    .Schedule<RebuildStaticCachedData>().Hourly();
```

For this example, `SendNightlyReportsEmailJob` and `SendPendingNotifications` will share a dedicated pipeline/thread.

`RebuildStaticCachedData` has it's own dedicated worker so it will not affect the other tasks if it does take a long time to run.

### Useful For

This is useful, for example, when using Coravel in a console application.

You can choose to scale-out your scheduled tasks however you feel is most efficient. Any super intensive tasks can be put onto their own worker and therefore won't cause the other scheduled tasks to lag behind!

## On App Closing

When your app is stopped, Coravel will wait until any running scheduled tasks are completed. This will keep your app running in the background - as long as the parent process is not killed.

## Examples

### Run a task once an hour only on Mondays.

```csharp
scheduler.Schedule(
    () => Console.WriteLine("Hourly on Mondays.")
)
.Hourly()
.Monday();
```

### Run a task every day at 1pm

```csharp
scheduler.Schedule(
    () => Console.WriteLine("Daily at 1 pm.")
)
.DailyAtHour(13); // Or .DailyAt(13, 00)
```

### Run a task on the first day of the month.

```csharp
scheduler.Schedule(
    () => Console.WriteLine("First day of the month.")
)
.Cron("0 0 1 * *") // At midnight on the 1st day of each month.
```

### Scheduling An Invocable That Sends A Daily Report

Imagine you have a "daily report" that you send out to users at the end of each day. What would be a simple, elegant way to do this?

Using Coravel's Invocables, Scheduler and Mailer all together can make it happen!

Take this sample class as an example:

```csharp
public class SendDailyReportsEmailJob : IInvocable
{
    private IMailer _mailer;
    private IUserRepository _repo;

    // Each param injected from the service container ;)
    public SendDailyReportsEmailJob(IMailer mailer, IUserRepository repo)
    {
        this._mailer = mailer;
        this._repo = repo;
    }

    public async Task Invoke()
    {
        var users = await this._repo.GetUsersAsync();

        foreach(var user in users)
        {
            var mailable = new NightlyReportMailable(user);
            await this._mailer.SendAsync(mailable);
        }        
    }
}
```

Now to schedule it:

```csharp
scheduler
    .Schedule<SendDailyReportsEmailJob>()
    .Daily();
```

Easy Peasy!
