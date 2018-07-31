# Coravel

[![CircleCI](https://circleci.com/gh/jamesmh/coravel/tree/master.svg?style=svg)](https://circleci.com/gh/jamesmh/coravel/tree/master)

__Note: Coravel is unstable as it's in the "early" stages of development. Once version 2 is released Coravel will be considered stable. Please use with this in mind :)__

Inspired by all the awesome features that are baked into the Laravel PHP framework - coravel seeks to provide additional features to .Net Core apps so you can get started building the "meat" of your app and avoid getting tied up configuring additional infrastructure (mail, cache, scheduling, etc.)

Right now, Coravel features:

## Full Docs

- [Task Scheduling](https://github.com/jamesmh/coravel/blob/master/Docs/Scheduler.md)
- [Queuing](https://github.com/jamesmh/coravel/blob/master/Docs/Queuing.md)
- [Caching](https://github.com/jamesmh/coravel/blob/master/Docs/Caching.md)
- [Mailing](https://github.com/jamesmh/coravel/blob/master/Docs/Mailing.md)
- [Coravel-Cli](https://github.com/jamesmh/coravel/blob/master/Docs/Cli.md)

## Requirements

Coravel is a .Net Core library. You must be including Coravel in an existing .Net Core application (version 2.1.0 +).

## Support

If you wish to encourage and support my efforts:

[![Buy Me A Coffee](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/gIPOyBD5N)

## Contributing

If you are fixing a typo in one file / place - issue a PR. 

Otherwise, please **always** create an issue first.

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

### 4. Mailing

### Basic Installation / Configuration

At minimum, we need to define our e-mail driver from our appsettings.json by adding this:

```json
"Coravel": {
  "Mail": {
    "Driver": "FileLog" // Basic driver that logs to a file ;)
  }
}
```

Phew! That was hard.

Next, in `ConfigureServices()` in `Startup.cs`:

```c#
services.AddMailer(this.Configuration); // Instance of IConfiguration.
```

Finally, create a `_ViewStart.cshtml` file in `~/Views/Mail`:

```c#
@{
    Layout = "~/Areas/Coravel/Pages/Mail/Template.cshtml";
}
```

This will automatically point your mail templates to one of Coravel's built-in templates ;)

### Creating Mailables

Coravel uses **Mailables** to send mail. Each Mailable is a C# class that represents a specific type of e-mail that you can send, such as
"New User Sign-up", "Completed Order", etc.

Consider this class and code comments highlighting the important basics:

```c#
using Coravel.Mail;
using App.Models;

namespace App.Mailables
{
    // Inherit from Mailable<T>
    public class NewUserViewMailable : Mailable<UserModel>
    {
        // Inject your model
        private UserModel _user;

        public NewUserViewMail(UserModel user) => this._user = user;

        // Build is called before sending mail.
        // This is where you can configure this mail for easy reusability!
        public override void Build()
        {
            // The UserModel public property "Email" is auto-bound
            // (and optionally a "Name" property also)
            this.To(this._user)
                .From("from@test.com")
                .View("~/Views/Mail/NewUser.cshtml", this._user);

            // View() is similar to calling View() from a controller.
        }
    }
}
```

Coravel will also use the name of your class (removing any postfix of "Mailable") to generate the subject. In this case, the subject would be "New User View". Of course, there is a `Subject()` method if needed.

### Using Razor Templates

`~/Views/Mail/NewUser.cshtml` might look like this:

```c#
@model App.Models.UserModel

@{
   ViewBag.Heading = "Welcome New User: " + Model.Name;
}

<p>
    Hi @Model.Name!
    @await Component.InvokeAsync("EmailLinkButton", new  { text = "click me", url = "www.google.com" })
</p>

@section links
{
    <a href="https://www.google.com">Google</a> | <a href="https://www.google.com">Google</a>
}
```

This file is using the Coravel supplied template that we specified in `_ViewStart.cshtml`.

### Sending Mail

Inject an instance of `Coravel.Mail.IMailer` and use the `SendAsync` method to send mail:

```c#
await this._mailer.SendAsync(new NewUserViewMailable(user));
```

A basic e-mail using Coravel's "plain" template might look like this:

![email sample](https://github.com/jamesmh/coravel/blob/master/Docs/email-sample.png)

### There's Much More! 

This is a quick look at Coravel's mailing! 

Check out the [full docs](https://github.com/jamesmh/coravel/blob/master/Docs/Mailing.md) to see all the available features in-depth!


