---
meta:
  - name: description
    content: Coravel is a near-zero config .NET Core library that makes Task Scheduling, Caching, Queuing, Mailing, Event Broadcasting (and more) a breeze!
  - name: keywords
    content: dotnet dotnetcore ".NET Core task scheduling" ".NET Core scheduler" ".NET Core framework" ".NET Core Queue" ".NET Core Queuing" ".NET Core Caching" Coravel
---

# Task Queuing

[[toc]]

Coravel allows zero-configuration queues (at run time).

Every 30 seconds, if there are any available items in the queue, they will be dequeued.

## Setup

In your `Startup` file, in the `ConfigureServices()` just do this:

```csharp
services.AddQueue();
```

That's it! This will automatically register the queue in your service container.

## How To Queue Tasks

In your controller that is using DI, inject a `Coravel.Queuing.Interfaces.IQueue`.

You use the `QueueTask()` method to add a task to the queue.

```csharp
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

```csharp
 this._queue.QueueAsyncTask(async() => {
    await Task.Delay(1000);
    Console.WriteLine("This was queued!");
 });
```

## Queue Invocables

To learn about creating and using invocables [see the docs.](/Invocables/)

To queue an invocable, use `QueueInvocable`:

```csharp
this._queue.QueueInvocable<GrabDataFromApiAndPutInDBInvocable>();
```

## Queue Event Broadcasting

> New in version 2.1

Event broadcasting is great - but what if your event listeners are doing some heavy / long-winded tasks?

Using `QueueBroadcast` you can queue an event to be broadcasted in the background so your app can continue to be responsive.

```csharp
// This will broadcast the event whenever the queue is consummed in the background.
this._queue.QueueBroadcast(new OrderCreated(orderId)); 
```

## Global Error Handling

> In version 1.9 `ConfigureQueue` was moved as an extension method of `IServiceProvider`.
> This allows Coravel's dependencies to be significantly slimmed down 👌

In the `Configure` method of your `Startup` file, first call `app.ApplicationServices.ConfigureQueue()` and further chain the `OnError()` method to register a global error handler.

```csharp
var provider = app.ApplicationServices;
provider
    .ConfigureQueue()
    .OnError(e =>
    {
        //.... handle the error
    });
```

## Adjusting The Consummation Delay

Normally, the queue will consume all of it's queued tasks every 30 seconds.
You can adjust this delay in the `appsettings.json` file.

```json
"Coravel": {
  "Queue": {
    "ConsummationDelay": 1
  }
}
```

## Logging Executed Task Progress

Coravel uses the `ILogger` .NET Core interface to allow logging task progress.

Enable logging by further chaining off the `ConfigureQueue` method, grabbing a logger from the service provider and passing it into `LogQueuedTaskProgress`:

```csharp
var provider = app.ApplicationServices;
provider
    .ConfigureQueue()
    .LogQueuedTaskProgress(provider.GetService<ILogger<IQueue>>());
```

## On App Closing

When your app is stopped, Coravel will consume any tasks remaining in the queue and/or wait until all long-running tasks are completed. This will keep your app running in the background - as long as the parent process is not killed.
