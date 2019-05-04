---
meta:
  - name: description
    content: Coravel is a near-zero config .NET Core library that makes Task Scheduling, Caching, Queuing, Mailing, Event Broadcasting (and more) a breeze!
  - name: keywords
    content: dotnet dotnetcore ".NET Core task scheduling" ".NET Core scheduler" ".NET Core framework" ".NET Core Queue" ".NET Core Queuing" ".NET Core Caching" Coravel
---

# Coravel CLI

[[toc]]

Coravel CLI is a .NET core global tool that gives you an _even quicker_ way to get started with Coravel!

## Installation

Install by running this command in your terminal:

`dotnet tool install --global coravel-cli`

## Install Coravel

You may use the CLI to install Coravel into an existing .NET core project.

In the root of your project, enter the command:

`coravel install`

This will install the required Nuget package(s) for you!

## Mailer

### Scaffold Mail Views

You can scaffold most of the mailer feature by using the CLI:

`coravel mail install`

### Create A New Mailable

You can generate a new mailable:

`coravel mail new [nameOfYourMailable]`

This will create both a new mailable class and view.

## Invocables

You may generate a new invocable:

`coravel invocable new [nameOfYourInvocable]`

The new class will be under `./Invocables`.

## Events And Listeners

You may generate events and corresponding listeners:

`coravel event new [eventName] [listenerName]`

If the event specified already exists, then the existing event will not be overwritten.

Therefore, you can issue the `coravel event new` command multiple times to generate multiple
listeners for the same event:

`coravel event new UserCreatedEvent SendUserCreatedEmailListener`

`coravel event new UserCreatedEvent StartBillingUserListener`

Given the event `UserCreatedEvent` already existed before issuing the commands above, only the new listeners will be created for you.

:::warning
Remember to register your listeners into the service container and register/subscribe your events and listeners.
:::
