---
meta:
  - name: description
    content: Coravel is a near-zero config .NET Core library that makes Task Scheduling, Caching, Queuing, Mailing, Event Broadcasting (and more) a breeze!
  - name: keywords
    content: dotnet dotnetcore ".NET Core task scheduling" ".NET Core scheduler" ".NET Core framework" ".NET Core Queue" ".NET Core Queuing" ".NET Core Caching" Coravel
---

# Invocables

[[toc]]

Invocables are ubiquitous classes that most of Coravel's features can leverage to make your code much easier to write, compose and maintain.

Each invocable represents a self-contained job within your system. 

Here's an example of scheduling an invocable:

```csharp
    scheduler.Schedule<ReIndexDatabase>()
      .DailyAtHour(01)
      .Weekday();
```

:::tip
It's handy to know about invocables up-front so you can use Coravel's features as intended!
:::

## Creating An Invocable

Creating an invocable uses the shared interface `Coravel.Invocable.IInvocable`.

Using .NET Core's dependency injection services, your invocables will have all their dependencies injected whenever they are executed.

### CLI

You may [use the Coravel Cli to generate a new invocable](/Cli/#invocables).

### Manually

1. Implement the interface `Coravel.Invocable.IInvocable` in your class.

2. In your invocable's constructor, inject any types that are available from your application's service provider.

3. Make sure that your invocable _itself_ is available in the service container.

```csharp
services.AddTransient<SendDailyStatsReport>();
services.AddTransient<SomeOtherInvocable>();
```

That's it!

## Examples

### #1 Generating A Daily Report And Emailing To Users

In this example, `SendDailyReportsEmailJob` is an invocable that was created by us. It handles getting data (via some repository that was injected via DI), generating an e-mail, etc.

![Coravel Invocable Sample](/img/scheduledailyreport.png)

A sample implementation of the `SendDailyReportsEmailJob` class might look something like this (which is using Coravel's Mailer to send email):

![Coravel Invocable Sample](/img/dailyreportinvocable.png)

### #2 Trigger Long Running Calculations In Background

You might have a use-case where an HTML button on an admin screen fires off an expensive
process which then stores the result in a database. You don't want your users waiting for it to finish, since it takes a while. 

Assuming you created an invocable `DoExpensiveCalculationAndStoreInDB`, you could run it like this:

![Coravel Invocable Sample](/img/queueexpensive.png)





