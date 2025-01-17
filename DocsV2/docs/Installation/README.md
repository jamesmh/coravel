---
meta:
  - name: description
    content: Coravel is a near-zero config .NET library that makes Task Scheduling, Caching, Queuing, Mailing, Event Broadcasting (and more) a breeze!
  - name: keywords
    content: dotnet ".NET task scheduling" ".NET scheduler" ".NET Queue" ".NET Queuing" ".NET Caching" Coravel
---

# Installation

[[toc]]

## GitHub

[You can visit the GitHub repo here.](https://github.com/jamesmh/coravel)

## Requirements

Coravel is a .NET Standard library designed for .NET Core apps.

Include Coravel in either an existing .NET Core application (version 2.1.0+) or another .NET Standard project(s).

## Installation

Coravel requires the nuget package `Coravel` to get started.

`dotnet add package coravel`

## Extras

### Coravel CLI

Alternatively, you may install Coravel using the [Coravel CLI](/Cli/).

Coravel CLI is a dotnet core tool that you can use as a global executable (similar to `npm` or `dotnet`) that gives you easy installs, scaffolding abilities, etc.

To install the CLI:

``` 
dotnet tool install --global coravel-cli
```

To install Coravel into an existing project with the CLI:

```
coravel install
```

### Coravel Pro

If you are using EF Core in your project, you might want to have a look at [Coravel Pro](/Pro/).
