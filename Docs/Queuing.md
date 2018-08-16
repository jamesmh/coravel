# Task Queuing

Coravel allows zero-configuration queues (at run time).

Every 30 seconds, if there are any available items in the queue, they will begin invocation.

_Note: The queue is a separate Hosted Service from the scheduler (i.e. they each run in isolation)_

## Setup

In your `Startup` file, in the `ConfigureServices()` just do this:

```c#
services.AddQueue();
```

That's it! This will automatically register the queue in your service container.

## How To Queue Tasks

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

## Queue Async Task

Use the `QueueAsyncTask` to queue up an async Task (which will run async whenever the queue is consumed).

```c#
 this._queue.QueueAsyncTask(async() => {
    await Task.Delay(1000);
    Console.WriteLine("This was queued!");
 });
```

## Global Error Handling

The `OnError()` extension method can be called after `AddQueue` to register a global error handler.

```c#
services
    .AddQueue()
    .OnError(e =>
    {
        //.... handle the error
    });
```

## Logging Executed Task Progress

Coravel uses the `ILogger` .Net Core interface to allow logging task progress.

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

Then enable logging:

```c#
services
    .AddQueue()
    .LogQueuedTaskProgress(Services.GetService<ILogger<IQueue>>());
```

The `LogQueuedTaskProgress()` method accepts an instance of `ILogger<IQueue>`, which is available through the service provider.

## On App Closing

When your app is stopped, Coravel will attempt to gracefully wait until the last moment and:

- Run the scheduler once last time
- Consume any tasks remaining in the queue

You shouldn't have to worry about loosing any queued items.

If your server was shutdown in a non-graceful way etc. (unplugged... etc.) then you may lose active queued tasks. But under normal circumstances, even when forcefully shutting down your app, Coravel will (in the background) handle this for you.

_Note: Queue persistence might be added in the future ;)_
