[![Netlify Status](https://api.netlify.com/api/v1/badges/5f511f8d-d256-4e4f-a21f-b7a444b4d4f9/deploy-status)](https://app.netlify.com/sites/coravel-docs/deploys)

[![BuiltWithDot.Net shield](https://builtwithdot.net/project/32/coravel/badge)](https://builtwithdot.net/project/32/coravel)
[![Nuget](https://img.shields.io/nuget/v/Coravel.svg)](https://www.nuget.org/packages/Coravel)
[![NuGet](https://img.shields.io/nuget/dt/Coravel.svg)](https://www.nuget.org/packages/Coravel)


<div align="center">
  <img src="./img/logo.png" style="max-width:200px" />
</div>

# Coravel

Coravel helps developers get their .NET applications up-and-running fast without compromising code quality. 

It makes advanced application features accessible and easy-to-use by giving you a simple, expressive and straightforward syntax.

![Coravel Scheduler](./img/scheduledailyreport.png)

## Features:

### Task Scheduling

Usually, you have to configure a cron job or a task via Windows Task Scheduler to get a single or multiple re-occurring tasks to run.

With Coravel, you can setup all your scheduled tasks in one place using a simple, elegant, fluent syntax - in code!

### Queuing

Coravel gives you a zero-configuration queue that runs in-memory to offload long-winded tasks to the background instead of making your users wait for their HTTP request to finish!

### Caching

Coravel provides you with an easy to use API for caching in your .NET Core applications.

By default, it uses an in-memory cache, but also has database drivers for more robust scenarios!

### Event Broadcasting

Coravel's event broadcasting helps you to build maintainable applications who's parts are loosely coupled!

### Mailing

E-mails are not as easy as they should be. Luckily for you, Coravel solves this by offering:

- Built-in e-mail friendly razor templates
- Simple and flexible mailing API
- Render your e-mails for visual testing
- Drivers supporting SMTP, local log file or BYOM ("bring your own mailer") driver
- Quick and simple configuration via `appsettings.json`
- And more!

## Official Documentation

[You can view the official docs here.](https://docs.coravel.net/Installation/)

## Samples

- [Using Coravel With EF Core](https://github.com/jamesmh/coravel/tree/master/Samples/EFCoreSample)
- [.NET Worker Service using Coravel's Task Scheduling](https://github.com/jamesmh/coravel/tree/master/Samples/WorkerServiceScheduler)

## Support Me

You can support my ongoing open-source work on [BuyMeACoffee](https://www.buymeacoffee.com/gIPOyBD5N).

## Coravel Pro

If you are building a .NET application with EF (Core), then you might want to look into [Coravel Pro](https://www.pro.coravel.net/). It is an admin panel & tools to make maintaining and managing your .NET app a breeze!

- Visual job scheduling & management
- Scaffold a CRUD UI for managing your EF entities
- Easily configure a dashboard to show health metrics (or whatever you want)
- Build custom tabular reports of your data
- And more!

## FAQ

### How is Coravel different from Hangfire?

Hangfire has been around for a while - before modern .NET (Core). It's a fantastic tool that has tons of features that Coravel doesn't. Notably: persistent queues, retry mechanisms, support for many storage drivers, etc.

However, Hangfire still (as of March 2023) does not natively support true `async/await` ([here](https://github.com/HangfireIO/Hangfire/issues/1658) and [here](https://github.com/HangfireIO/Hangfire/issues/401)). This means that using Hangfire within a web application, for example, won't be as efficient as it could be when using threads that perform I/O operations.

Coravel was created with modern C# and .NET primitives in mind - such as `async/await` and .NET's built-in dependency injection utilities. This means that Coravel can be easier to configure and will be very efficient with / won't hog threads that your web application needs to respond to incoming HTTP requests.

### How is Coravel different from Quartz?

Quartz is an older Java library ported to .NET. It still doesn't hook into the modern .NET dependency injection tooling well. Some think that Coravel's APIs are much more succinct and understandable.

For example, compare [this sample](https://www.quartz-scheduler.net/documentation/quartz-3.x/quick-start.html#starting-a-sample-application) from their documentation with how working with Coravel is (e.g. you don't need to understand how to "start" and "stop" Coravel's scheduler, but you do have to manually work with the Quartz scheduler).

### Does Coravel support persisting queued jobs to storage in case my application goes down?

No. At least, not yet. 

Coravel processes queued items in-memory. When your application goes down it won't allow the application to shutdown until all items are processed.

### Does Coravel support retry mechanisms?

Coravel's philosophy has been to work well with other .NET primitives - which means that using other libraries is easy. 

Coravel doesn't support retry mechanisms internally because I am very careful not to bloat Coravel with things that aren't necessary. I want to keep Coravel focused on what it does best (e.g. "I need job scheduling, queuing, etc. without requiring extra infrastructure and complicated configuration").

For example, you can use [Polly](https://github.com/App-vNext/Polly) within your invocables to do retries. Some people will configure a base class that inherits from `IInvocable` that has retries built-in. 

### Does Coravel support distributed locking?

No. However, this can again be achieved by using a battle tested distributed locking library like [DistributedLock](https://github.com/madelson/DistributedLock). You might create an invocable's `Invoke()` like this:

```csharp
public class TestInvocable : IInvocable
{
  private ApplicationDbContext _context;
  private IDistributedLockProvider _distributedlock;

  public TestInvocable(ApplicationDbContext context, IDistributedLockProvider distributedlock)
  {
    this._context = context;
    this._distributedlock = distributedlock;
  }

  public async Task Invoke()
  {
    await using (await this._distributedlock.AcquireAsync())
    {
      await this._context.Test.AddAsync(new TestModel() { Name = "test name" });
      await this._context.SaveChangesAsync();
    }
  }
}
```
