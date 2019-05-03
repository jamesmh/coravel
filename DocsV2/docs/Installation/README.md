---
meta:
  - name: description
    content: Coravel is a near-zero config .NET Core library that makes Task Scheduling, Caching, Queuing, Mailing, Event Broadcasting (and more) a breeze!
  - name: keywords
    content: dotnet dotnetcore ".NET Core task scheduling" ".NET Core scheduler" ".NET Core framework" ".NET Core Queue" ".NET Core Queuing" ".NET Core Caching" Coravel
---

# Installation

[[toc]]

## Requirements

Coravel is a .NET Standard library designed for .NET Core apps. You must be including Coravel into either an existing .NET Core application (version 2.1.0+) or within another .NET Standard project(s).

## Installation

Coravel requires the nuget package `Coravel` to get started.

`dotnet add package coravel`

## Coravel CLI

Alternatively, you may install Coravel using the [Coravel CLI](/Cli/).

Coravel CLI is a dotnet core tool that you can use as a global executable (similar to `npm` or `dotnet` etc.) that gives you easy installs, scaffolding abilities, etc.

Install the tool:

``` 
dotnet tool install --global coravel-cli
```

Then install coravel into the current project:

```
coravel install
```

## Pro

If you are using EF Core in your project, you might want to have a look at [Coravel Pro](/Pro/).