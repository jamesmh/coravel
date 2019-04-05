[![CircleCI](https://circleci.com/gh/jamesmh/coravel/tree/master.svg?style=svg)](https://circleci.com/gh/jamesmh/coravel/tree/master)
[![BuiltWithDot.Net shield](https://builtwithdot.net/project/32/coravel/badge)](https://builtwithdot.net/project/32/coravel)
![Nuget](https://img.shields.io/nuget/v/Coravel.svg)
![NuGet](https://img.shields.io/nuget/dt/Coravel.svg)

<div align="center">
  <img src="./Docs/img/logo.png" style="max-width:200px" />
</div>

# Coravel

Coravel focuses on helping developers get their web applications up-and-running fast - without compromising code quality. It makes what are usually advanced features accessible and easy-to-use - without needing to install any extra 3rd-party infrastructure.

Build .NET Core apps using a simple, expressive and straightforward syntax that lets you focus on **your app - "batteries included!"** 

![Coravel Scheduler](./Docs/img/scheduledailyreport.png)

## Used By

<p align="center">
  <a href="https://www.builtwithdot.net">
    <img src="./Docs/img/used-by/builtwithdotnet.jpg" alt="BuiltWithDotNet" style="width:60%;" />
  </a>
</p>
<p align="center"><b>BuiltWithDot.Net</b></p>

## Features / Docs

- [Task Scheduling](./Docs/Scheduler.md)
- [Queuing](./Docs/Queuing.md)
- [Caching](./Docs/Caching.md)
- [Mailing](./Docs/Mailing.md)
- [Invocables](./Docs/Invocables.md)
- [Events](./Docs/Events.md)
- [Coravel CLI](./Docs/Cli.md)

## Requirements

Coravel is a .NET Standard library designed for .NET Core apps. You must be including Coravel into either an existing .NET Core application (version 2.1.0+) or within another .NET Standard project(s).

### Coravel CLI

Use the [Coravel CLI](./Docs/Cli.md) to get started!

Coravel CLI is a dotnet core tool that you can use as a global executable (similar to `npm` or `dotnet` etc.) that gives you easy installs, scaffolding abilities, etc.

Install the tool:

```
dotnet tool install --global coravel-cli
```

### Installation

Coravel requires the nuget package `Coravel` to get started.

Alternatively, you may install Coravel using the CLI (which will install the package for you):

```
coravel install
```

Done!

### What Do I Do Next?

Check out the top of this readme for an index of Coravel's features, each linking to the appropriate docs!

## Contributing

If you are fixing a typo in one file / place - issue a PR.

Otherwise, please **always** create an issue first ;)

## Coravel Pro

Check out [Coravel Pro](https://github.com/jamesmh/coravelpro) for a professional admin panel and visual tools that seamlessly integrate into your .NET Core apps.

Includes:

- Database persisted scheduling
- A beautiful UI to manage your jobs/invocables
- Health metrics dashboard
- Easily configured tabular reports of your data (supports aggregation/complex projections)
