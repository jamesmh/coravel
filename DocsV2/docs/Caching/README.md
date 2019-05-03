---
meta:
  - name: description
    content: Coravel is a near-zero config .NET Core library that makes Task Scheduling, Caching, Queuing, Mailing, Event Broadcasting (and more) a breeze!
  - name: keywords
    content: dotnet dotnetcore ".NET Core task scheduling" ".NET Core scheduler" ".NET Core framework" ".NET Core Queue" ".NET Core Queuing" ".NET Core Caching" Coravel
---

# Caching

[[toc]]

Coravel provides you with an easy to use API for caching in your .Net Core applications.

## Under The Hood

Coravel's cache is basically a wrapper for .Net Core's built-in caching. Coravel provides you with some extra features and a simpler syntax when doing typical/common caching operations.

## Setup 

Install Coravel, if not installed.

Next, in `Startup.cs`, put this in `ConfigureServices()`:

```csharp
services.AddCache();
```

This will enable in-memory (RAM) caching.

Then, inject `ICache` (from `Coravel.Cache.Interfaces`) via dependency injection. 

```csharp
private ICache _cache;

public CacheController(ICache cache)
{
    this._cache = cache;
}
```

Now, in your controller/injectable class, you may use Coravel's caching methods.

## Available Methods

### Remember

`Remember` will remember your cached item for the duration you specify. 

`Remember` requires that you specify:
- A cache key for this specific item
- A method that returns the data you want cached (of any type)
- A `TimeSpan` to indicate how long the item will be cached 

For example:

```csharp
string BigDataLocalFunction() 
{
    return "Some Big Data";
};

this._cache.Remember("BigDataCacheKey", BigDataLocalFunction, TimeSpan.FromMinutes(10));
```

Pretty simple? 

_P.S. It is always recommended that you not use hardcoded cache keys._

### RememberAsync

It's `Remember`, but async:

```csharp
async SomeType BigDataLocalFunctionAsync() 
{
    // ... Doing some stuff ... 
    return await SomeCostlyDbCall();
};

await this._cache.RememberAsync("BigDataCacheKey", BigDataLocalFunctionAsync, TimeSpan.FromMinutes(10));
```

### Forever

Similar to `Remember`, but your item will be cached indefinitely (see `Forget` and `Flush` for cache clearing).

```csharp
this._cache.Forever("BigDataCacheKey", BigDataLocalFunction);
```

### ForeverAsync

It's `Forever`, but async:

```csharp
await this._cache.ForeverAsync("BigDataCacheKey", BigDataLocalFunctionAsync);
```

### Flush

`Flush` will clear your entire cache.

```csharp
this._cache.Flush();
```

### Forget

`Forget` will clear a specific cache entry by key.

```csharp
this._cache.Forget("BigDataCacheKey");
```

## Extending Coravel With Custom Drivers

If you wish, you can create your own cache driver that extends `Coravel.Cache.Interfaces.ICache`. Maybe you want to use Coravel but use a Redis store? 

First, implement a class that extends the `ICache` interface.

Next, to register your driver to be used, just pass it into the `AddCache` method:

```csharp
services.AddCache(new RedisCache());

// Or, if you need the service provider to create your object:

services.AddCache(provider => new RedisCache(provider.GetService<ISomeRegisteredInterface>()));
```



