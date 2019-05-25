---
meta:
  - name: description
    content: Coravel is a near-zero config .NET Core library that makes Task Scheduling, Caching, Queuing, Mailing, Event Broadcasting (and more) a breeze!
  - name: keywords
    content: dotnet dotnetcore ".NET Core task scheduling" ".NET Core scheduler" ".NET Core framework" ".NET Core Queue" ".NET Core Queuing" ".NET Core Caching" Coravel
---

# Caching

[[toc]]

Coravel provides you with an easy to use API for caching in your .NET Core applications.

By default, it uses an in-memory cache.

Coravel also provides some [database drivers](#database-drivers) for more robust scenarios.

## Config 

In `Startup.ConfigureServices()`:

```csharp
services.AddCache();
```

This will enable in-memory (RAM) caching.

To use caching, inject `Coravel.Cache.Interfaces.ICache` via dependency injection. 

```csharp
private ICache _cache;

public CacheController(ICache cache)
{
    this._cache = cache;
}
```

## Methods

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

Similar to `Remember`, but your item will be cached indefinitely.

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

## Database Drivers

By default, an in-memory caching driver is used. Coravel also offers drivers that will cache your data to a database!

::: tip
This allows multiple servers or application instances (e.g. load-balancing, etc.) to share the same cache. It can also alleviate the extra memory required by your application instance(s) when using the in-memory driver.
:::

### SQL Server

Install the NuGet package `Coravel.Cache.SQLServer`.

Next, in `Startup.cs` in the `ConfigureServices()` method:

```csharp
services.AddSQLServerCache(connectionString);
```

The driver will automatically create the table `dbo.CoravelCacheStore` to be used for caching.

### PostgreSQL

Install the NuGet package `Coravel.Cache.PostgreSQL`.

Next, in `Startup.cs` in the `ConfigureServices()` method:

```csharp
services.AddPostgreSQLCache(connectionString);
```

The driver will automatically create the table `public.CoravelCacheStore` to be used for caching.

## Custom Drivers

If you wish, you can create your own cache driver that extends `Coravel.Cache.Interfaces.ICache`. Maybe you want to use start using a Redis store? 

First, implement a class that extends the `ICache` interface.

Next, to register your driver to be used, just pass it into the `AddCache` method:

```csharp
services.AddCache(new RedisCache());

// Or, if you need the service provider to create your object:
services.AddCache(provider => new RedisCache(provider.GetService<ISomeRegisteredInterface>()));
```



