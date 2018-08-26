# Task Queuing

Coravel allows zero-configuration queues (at run time).

Every 30 seconds, if there are any available items in the queue, they will be dequeued.

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

## Queue Invocables

Coravel has a shared interface `Coravel.Invocable.IInvocable` that allows you to define specific actions or jobs within your system. Using .Net Core's dependency injection services, your invocables will have all their dependencies injected whenever they are executed.

### Create an Invocable

To create an invocable, implement the interface above in your class.

In your invocable's constructor, inject any types that are available from your application's service container.

Make sure that your invocable _itself_ is available in the service container.

### Queue It

To queue it, use `QueueInvocable`:

```c#
this._queue.QueueInvocable<GrabDataFromApiAndPutInDBInvocable>();
```

_Note: Coravel's scheduler can schedule invocables too._


## Global Error Handling

In the `Startup.Configure` method, first call `app.ConfigureQueue()` and further chain the `OnError()` method to register a global error handler.

```c#
app
    .ConfigureQueue()
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

Then enable logging by further chaining off the `ConfigureQueue` method:

```c#
app
    .ConfigureQueue()
    .LogQueuedTaskProgress(Services.GetService<ILogger<IQueue>>());
```

The `LogQueuedTaskProgress()` method accepts an instance of `ILogger<IQueue>`, which is available through the service provider.

## On App Closing

When your app is stopped, Coravel will attempt to gracefully wait until the last moment and consume any tasks remaining in the queue.

You shouldn't have to worry about loosing any queued items.

If your server was shutdown in a non-graceful way etc. (unplugged... etc.) then you may lose active queued tasks. But under normal circumstances, even when forcefully shutting down your app, Coravel will (in the background) handle this for you.

_Note: Queue persistence might be added in the future ;)_
