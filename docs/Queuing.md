# Feature: Task Queing

Coravel allows zero-configuration queues (at run time). The queue hooks into the scheduling mechanism (although that is handled for you).

The scheduler checks for scheduled tasks every minutes. If there are any available items in the queue, then it will invoke them all.

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

## Handling Errors

Since the queue is hooked into the scheduling mechanism, the `OnError()`extension method that corvel provides for your `IServiceCollection` will be used for your queued tasks.

Again, you may use proper error handling in your tasks too.

## On App Closing

When your app is stopped, coravel will attempt to gracefully wait until the last moment and:

- Run the scheduler once last time
- Consume any tasks remaining in the queue

You shouldn't have to worry about loosing any queued items.

If your server was shutdown in a non-graceful way etc. (unplugged... etc.) then you may lose active queued tasks. But under normal circumstances, even when forcefully shutting down your app, coravel will (in the background) handle this for you.
