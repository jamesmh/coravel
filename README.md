# Coravel

Inspired by all the awesome features that are baked into the Laravel PHP framework - coravel seeks to provide additional features that .Net Core lacks like:

- Task Scheduling
- Queuing
- Mailer
- Command line tools integrated with coraval features
- More???

## Quick-Start

Add the nuget package `Coravel` to your .NET Core app. Done!

### Task Scheduling

Tired of using cron and Windows Task Scheduler? Want to just use something easy that ties into your existing code?

In `Startup.cs`, put this in `ConfigureServices()`:

```c#
services.AddScheduler(scheduler =>
    {
        scheduler.Schedule(
            () => Console.WriteLine("Run at 1pm utc during week days.")
        )
        .DailyAt(13, 00);
        .Weekday();
    }
);
```

Easy enough? Look at the documentation to see what methods are available to you!

### Task Queuing

Tired of having to install other systems to get a queuing system to work? Tired of using databases to issue queued tasks?

In `Startup.cs`, put this in `ConfigureServices()`:

```c#
services.AddQueue();
```

Voila! Too hard?

To append to the queue, inject `IQueue` into your controller (or wherever injection occurs):

```c#
using Coravel.Queuing.Interfaces; // Don't forget this!

// Now inside your MVC Controller...
IQueue _queue;

public HomeController(IQueue queue) {
    this._queue = queue;
}
```

And then call:

```c#
this._queue.QueueTask(() =>
    Console.WriteLine("This was queued!")
);
```

Now you have a fully functional queue!

## Feature: Task Scheduling

Usually, you have to configure a cron job or a task via Windows Task Scheduler to get a single or multiple re-occuring tasks to run. With coravel you can setup all your scheduled tasks in one place! And it's super easy to use!

### Initial Setup

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

### How Does It Work?

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

### Error Handling

Any tasks that throw errors __will just be skipped__ and the next task in line will be invoked.

If you want to catch errors and do something specific with them you may use the `OnError()` extensions method on your `IServiceCollection`.

```c#
services.OnError((exception) => doSomethingWithException(exception));
```

You can, of course, add error handling inside your specific tasks too.

### Scheduling Tasks

After you have called the `Schedule()` method on the `Scheduler`, you can begin to configure the schedule constraints of your task.

#### Intervals

First, methods to apply interval constraints are available.

##### Basic Intervals

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

##### Intervals With Time Contraints

These methods allow you specify an interval and a time constraint so that your scheduling is more specific and consistent.

Please note that the scheduler is using _UTC_ time. So, for example, using `DailyAt(13, 00)` will run your task daily at 1pm _UTC_ time.

- `HourlyAt(int minute)`
- `DailyAtHour(int hour)`
- `DailyAt(int hour, int minute)`

#### Day Constraints

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

## Feature: Task Queing

Coravel allows zero-configuration queues (at run time). The queue hooks into the scheduling mechanism (although that is handled for you).

The scheduler checks for scheduled tasks every minutes. If there are any available items in the queue, then it will invoke them all.

### Setup

In your `Startup` file, in the `ConfigureServices()` just do this:

```c#
services.AddQueue();  
```

That's it! This will automatically register the queue in your service container.

### How To Queue Tasks

In your controller that is using DI, inject a `Coravel.Queuing.Interfaces.IQueue`.

You use the `QueueTask()` method to add a task to the queue.

```c#
IQueue _queue;

public HomeController(IQueue queue) {
    this._queue = queue;
}

//... Further down ...

public IActionResult QueueTask() {
    // Call .QueueTask() to add item to the queue!
    this._queue.QueueTask(() => Console.WriteLine("This was queued!"));
    return Ok();
}
```

### Handling Errors

Since the queue is hooked into the scheduling mechanism, the `OnError()`extension method that corvel provides for your `IServiceCollection` will be used for your queued tasks.

Again, you may use proper error handling in your tasks too.

## On App Closing

When your app is stopped, coravel will attempt to gracefully wait until the last moment and:

- Run the scheduler once last time
- Consume any tasks remaining in the queue

You shouldn't have to worry about loosing any queued items.

If your server was shutdown in a non-graceful way etc. (unplugged... etc.) then you may lose active queued tasks. But under normal circumstances, even when forcefully shutting down your app, coravel will (in the background) handle this for you.
