# Coravel

[![CircleCI](https://circleci.com/gh/jamesmh/coravel/tree/master.svg?style=svg)](https://circleci.com/gh/jamesmh/coravel/tree/master)

**Note: Coravel is unstable as it's in the "early" stages of development. Once version 2 is released Coravel will be considered stable. Please use with this in mind :)**

Inspired by all the awesome features that are baked into the Laravel PHP framework - Coravel provides tools for .Net Core apps so you can get started building **your app** faster!

## Who Is Coravel For?

Coravel's philosophy is basically that you should be focusing on building your app and bringing value to your business - not repetitive installation and configuration.

If you have asked yourself one of these questions, then Coravel might be for you:

- What mailing library should I use? Where do I put all that repetitive logic?
- Why is it so hard to just _simply use the code I already have_ and run it once a day at 1 a.m.?
- Do I need to learn how to use RabbitMQ or Redis when I just want to queue sending my e-mails in the background?
- I just want to cache my expensive query results and refresh them every 10 minutes - shouldn't there be an easy way to do that?
- Why does it take so long to actually get to the point where I'm _finally building my app_?

If you are in need of a way to get up-and-running quickly and start building **your app**, instead of all the infrastructure that goes along with a typical web app - and the overly verbose code that comes along for the ride - then Coravel might be for you!

## Features / Docs

- [Task Scheduling](https://github.com/jamesmh/coravel/blob/master/Docs/Scheduler.md)
- [Queuing](https://github.com/jamesmh/coravel/blob/master/Docs/Queuing.md)
- [Caching](https://github.com/jamesmh/coravel/blob/master/Docs/Caching.md)
- [Mailing](https://github.com/jamesmh/coravel/blob/master/Docs/Mailing.md)
- [Coravel-Cli](https://github.com/jamesmh/coravel/blob/master/Docs/Cli.md)

## Requirements

Coravel is a .Net Core library. You must be including Coravel in an existing .Net Core application (version 2.1.0 +).

### Coravel-Cli

Use the [Coravel Cli](https://github.com/jamesmh/coravel/blob/master/Docs/Cli.md) to get started!

Coravel Cli is a dotnet core tool that you can use as a global executable (similar to `npm` or `dotnet` etc.) that gives you easy installs, scaffolding abilities, etc.

Install the tool:

```
dotnet tool install --global coravel-cli
```

### Installation

Coravel requires a few dependencies that the cli will manage for you.

To install Coravel, run:

```
coravel install
```

Done!

### What Do I Do Next?

Check out the top of this readme for an index of Coravel's features, each linking to the appropriate docs!

## Contributing

If you are fixing a typo in one file / place - issue a PR.

Otherwise, please **always** create an issue first ;)

## Support

If you wish to encourage and support my efforts:

[![Buy Me A Coffee](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/gIPOyBD5N)
