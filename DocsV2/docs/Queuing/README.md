---
meta:
  - name: description
    content: Coravel is a near-zero config .NET Core library that makes Task Scheduling, Caching, Queuing, Mailing, Event Broadcasting (and more) a breeze!
  - name: keywords
    content: dotnet dotnetcore ".NET Core task scheduling" ".NET Core scheduler" ".NET Core framework" ".NET Core Queue" ".NET Core Queuing" ".NET Core Caching" Coravel
---

# Task Queuing

[[toc]]

Coravel gives you a zero-configuration queue that runs in-memory. 

This is useful to offload long-winded tasks to the background instead of making your users wait for their HTTP request to finish.

## Config

In your `Startup` file, in the `ConfigureServices()`:

```csharp
services.AddQueue();
```

That's it!

## Essentials

### Setup

Inject an instance of the `Coravel.Queuing.Interfaces.IQueue` interface into your controller, etc.

```csharp
IQueue _queue;

public HomeController(IQueue queue) {
    this._queue = queue;}
```

### Queuing Invocables

To queue an invocable, use `QueueInvocable`:

```csharp
this._queue.QueueInvocable<GrabDataFromApiAndPutInDBInvocable>();
```

:::tip
Queuing invocables is the recommended way to use Coravel's queuing.

To learn about creating and using invocables [see here.](/Invocables/)
:::

### Queuing An Async Task

Use the `QueueAsyncTask` to queue up an async task:

```csharp
 this._queue.QueueAsyncTask(async() => {
    await Task.Delay(1000);
    Console.WriteLine("This was queued!");
 });
```

### Queuing A Synchronous Task

You use the `QueueTask()` method to add a  task to the queue.

```csharp
public IActionResult QueueTask() {
    this._queue.QueueTask(() => Console.WriteLine("This was queued!"));
    return Ok();
}
```

### Queuing An Event Broadcast

Event broadcasting is great - but what if your event listeners are doing some heavy / long-winded tasks? You don't want that to happen on the same thread as your HTTP request!

By using `QueueBroadcast`, you can queue an event to be broadcasted in the background.

```csharp
this._queue.QueueBroadcast(new OrderCreated(orderId)); 
```

## Extras

### Global Error Handling

In the `Configure` method of your `Startup` file:

```csharp
var provider = app.ApplicationServices;
provider
    .ConfigureQueue()
    .OnError(e =>
    {
        //... handle the error
    });
```

### Adjusting The Consummation Delay

Normally, the queue will consume all of it's queued tasks every 30 seconds.
You can adjust this delay in the `appsettings.json` file.

```json
"Coravel": {
  "Queue": {
    "ConsummationDelay": 1
  }
}
```

### Logging Executed Task Progress

Coravel uses the `ILogger` .NET Core interface to allow logging task progress:

```csharp
var provider = app.ApplicationServices;
provider
    .ConfigureQueue()
    .LogQueuedTaskProgress(provider.GetService<ILogger<IQueue>>());
```

## On App Closing

When your app is stopped, Coravel will immediately begin consuming any remaining tasks and wait until they are completed. 

This will keep your app running in the background - as long as the parent process is not killed.
