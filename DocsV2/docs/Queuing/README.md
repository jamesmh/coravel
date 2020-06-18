---
meta:
  - name: description
    content: Coravel is a near-zero config .NET Core library that makes Task Scheduling, Caching, Queuing, Mailing, Event Broadcasting (and more) a breeze!
  - name: keywords
    content: dotnet dotnetcore ".NET Core task scheduling" ".NET Core scheduler" ".NET Core framework" ".NET Core Queue" ".NET Core Queuing" ".NET Core Caching" Coravel
---

# Queuing

[[toc]]

Coravel gives you a zero-configuration queue that runs in-memory. 

This is useful to offload long-winded tasks to the background instead of making your users wait for their HTTP request to finish.

## Setup

In your `Startup` file, in the `ConfigureServices()`:

```csharp
services.AddQueue();
```

The, you inject an instance of the `Coravel.Queuing.Interfaces.IQueue` interface into your controller, etc.

```csharp
IQueue _queue;

public HomeController(IQueue queue) {
    this._queue = queue;
}
```

## Queuing Jobs
### Queuing Invocables

To queue an invocable, use `QueueInvocable`:

```csharp
this._queue.QueueInvocable<GrabDataFromApiAndPutInDBInvocable>();
```

:::tip
Queuing invocables is the recommended way to use Coravel's queuing.

To learn about creating and using invocables [see here.](/Invocables/)
:::

### Queue With A Payload

Many times you want to queue a background job and also supply a payload/parameters.

For example, you might have an invocable `SendWelcomeUserEmailInvocable`. However, you need to supply a specific user's information so that the correct user will receive the email!

First, add the `IInvocableWithPayload<T>` interface to your existing invocable:

```csharp
                                                         // This one 👇
public class SendWelcomeUserEmailInvocable : IInvocable, IInvocableWithPayload<UserModel>
{
  // This is the implementation of the interface 👇
  public UserModel Payload { get; set; }

  /* Constructor, etc. */

  public async Task Invoke()
  {
    // `this.Payload` will be available to use now.
  }
}
```

To queue this invocable, use the `QueueInvocableWithPayload` method:

```csharp
var userModel = await _userService.Get(userId);
queue.QueueInvocableWithPayload<SendWelcomeUserEmailInvocable, UserModel>(userModel);
```

Now your job will be queued to execute in the background with the specific data it needs to run!

### Queuing Async

Use the `QueueAsyncTask` to queue up an async task:

```csharp
 this._queue.QueueAsyncTask(async() => {
    await Task.Delay(1000);
    Console.WriteLine("This was queued!");
 });
```

### Queuing Synchronously

You use the `QueueTask()` method to add a  task to the queue.

```csharp
public IActionResult QueueTask() {
    this._queue.QueueTask(() => Console.WriteLine("This was queued!"));
    return Ok();
}
```

### Queue Event Broadcast

Event broadcasting is great - but what if your event listeners are doing some heavy / long-winded tasks? You don't want that to happen on the same thread as your HTTP request!

By using `QueueBroadcast`, you can queue an event to be broadcasted in the background.

```csharp
this._queue.QueueBroadcast(new OrderCreated(orderId)); 
```

## Queuing Cancellable Invocables

Sometimes you have a long-running invocable that needs the ability to be cancelled (manually by you or by the system when the application is being shutdown).

By using `QueueCancellableInvocable` you can build invocables that will have this ability!

**First**, add the `Coravel.Queuing.Interfaces.ICancellableTask` to your invocable and implement the property:

`CancellationToken Token { get; set; }`

**Next**, in your `Invoke` method, you will have access to check if the token has been cancelled.

For example:

```csharp
while(!this.Token.IsCancellationRequested)
{
  await ProcessNextRecord();
}
```

**Then**, to capture a token:

```csharp
 var token = queue.QueueCancellableInvocable<MyCancellableInvocable>();
 // Do something with the token.
```

:::warning
Coravel will automatically dispose all `TokenCancellationSource` objects associated with invocables that have been consumed by the queue. 

Also, when the application is being shutdown, Coravel will also cancel _all_ tokens to make sure your invocables can stop their work and allow a graceful shutdown.

If you are manually cancelling/handling tokens then you'll have to code around the potential for your tokens to have been cancelled and even disposed.
:::

## Global Error Handling

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

## Adjusting Consummation Delay

Normally, the queue will consume all of it's queued tasks every 30 seconds.
You can adjust this delay in the `appsettings.json` file.

```json
"Coravel": {
  "Queue": {
    "ConsummationDelay": 1
  }
}
```

## Logging Task Progress

Coravel uses the `ILogger` .NET Core interface to allow logging task progress:

```csharp
var provider = app.ApplicationServices;
provider
    .ConfigureQueue()
    .LogQueuedTaskProgress(provider.GetService<ILogger<IQueue>>());
```

## On App Shutdown

When your app is stopped, Coravel will immediately begin consuming any remaining tasks and wait until they are completed. 

This will keep your app running in the background - as long as the parent process is not killed.
