# Coravel

[![CircleCI](https://circleci.com/gh/jamesmh/coravel/tree/master.svg?style=svg)](https://circleci.com/gh/jamesmh/coravel/tree/master)

__Note: Coravel is unstable as it's in the "early" stages of development. Once version 2 is released Coravel will be considered stable. Please use with this in mind :)__

Inspired by all the awesome features that are baked into the Laravel PHP framework - coravel seeks to provide additional features that .Net Core lacks like:

- Task Scheduling
- Queuing
- Caching
- Mailer [TBA]
- Command line tools integrated with coraval features [TBA]
- More?

## Full Docs

- [Task Scheduling](https://github.com/jamesmh/coravel/blob/master/Docs/Scheduler.md)
- [Queuing](https://github.com/jamesmh/coravel/blob/master/Docs/Queuing.md)
- [Caching](https://github.com/jamesmh/coravel/blob/master/Docs/Caching.md)

## Requirements

Coravel is a .Net Core library. You must be including Coravel in an existing .Net Core application (version 2.1.0 +).

## Support

If you wish to encourage and support my efforts:

[![Buy Me A Coffee](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/gIPOyBD5N)

## Quick-Start

Add the nuget package `Coravel` to your .NET Core app. Done!

### 1. Scheduling Tasks

Tired of using cron and Windows Task Scheduler? Want to use something easy that ties into your existing code?

In `Startup.cs`, put this in `ConfigureServices()`:

```c#
services.AddScheduler(scheduler =>
    {
        scheduler.Schedule(
            () => Console.WriteLine("Run at 1pm utc during week days.")
        )
        .DailyAt(13, 00)
        .Weekday();
    }
);
```

For async tasks you may use `ScheduleAsync()`:

```c#
scheduler.ScheduleAsync(async () =>
{
    await Task.Delay(500);
    Console.WriteLine("async task");
})
.EveryMinute();
```

Easy enough? Look at the documentation to see what methods are available!

### 2. Task Queuing

Tired of having to install and configure other systems to get a queuing system up-and-running? Look no further!

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

Or, for async tasks:

```c#
this._queue.QueueAsyncTask(async () =>
{
    await Task.Delay(100);
    Console.WriteLine("This was queued!")
});
```

Now you have a fully functional queue!

### 3. Caching

Wish there was a simple syntax for enabling caching in your .net core app? Coravel gives you a super simple API to enable caching!

In `Startup.cs`, put this in `ConfigureServices()`:

```c#
services.AddCache();
```

Phew! That was hard!

Next, you need to inject `ICache` (from `Coravel.Cache.Interfaces`) via dependency injection. 

```c#
private ICache _cache;

public CacheController(ICache cache)
{
    this._cache = cache;
}
```

To cache an object that is refreshed every 10 minutes, for example, Coravel provides the `Remember()` method: 

```c#
string BigDataLocalFunction() 
{
    return "Some Big Data";
};

this._cache.Remember("BigDataCacheKey", BigDataLocalFunction, TimeSpan.FromMinutes(10));
```

To cache an item forever:

```c#
this._cache.Forever("BigDataCacheKey", BigDataLocalFunction);
```

There are more methods for clearing your cache, async methods, etc. See the [full docs](https://github.com/jamesmh/coravel/blob/master/Docs/Caching.md) for more info.

