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

To learn about creating and using invocables [see the docs.](./Invocables.md)

To queue an invocable, use `QueueInvocable`:

```c#
this._queue.QueueInvocable<GrabDataFromApiAndPutInDBInvocable>();
```

## Global Error Handling

> In version 1.9 `ConfigureQueue` was moved as an extension method of `IServiceProvider`.
> This allows Coravel's dependencies to be significantly slimmed down 👌

In the `Configure` method of your `Startup` file, first call `app.ApplicationServices.ConfigureQueue()` and further chain the `OnError()` method to register a global error handler.

```c#
var provider = app.ApplicationServices;
provider
    .ConfigureQueue()
    .OnError(e =>
    {
        //.... handle the error
    });
```

## Logging Executed Task Progress

Coravel uses the `ILogger` .NET Core interface to allow logging task progress.

Enable logging by further chaining off the `ConfigureQueue` method, grabbing a logger from the service provider and passing it into `LogQueuedTaskProgress`:

```c#
var provider = app.ApplicationServices;
provider
    .ConfigureQueue()
    .LogQueuedTaskProgress(provider.GetService<ILogger<IQueue>>());
```

## On App Closing

When your app is stopped, Coravel will attempt to gracefully wait until the last moment and consume any tasks remaining in the queue.

You shouldn't have to worry about loosing any queued items.

If your server was shutdown in a non-graceful way etc. (unplugged... etc.) then you may lose active queued tasks. But under normal circumstances, even when forcefully shutting down your app, Coravel will (in the background) handle this for you.

_Note: Queue persistence might be added in the future ;)_
