# Caching

Coravel provides you with an easy to use API for caching in your .Net Core applications.

## Under The Hood

Coravel's cache is basically a wrapper for .Net Core's built-in caching. Coravel provides you with some extra features and a simpler syntax when doing typical/common caching operations.

## Setup

Install Coravel, if not installed.

Next, in `Startup.cs`, put this in `ConfigureServices()`:

```c#
services.AddCache();
```

This will enable in-memory (RAM) caching.

Then, inject `ICache` (from `Coravel.Cache.Interfaces`) via dependency injection.

```c#
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

```c#
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

```c#
async SomeType BigDataLocalFunctionAsync()
{
    // ... Doing some stuff ...
    return await SomeCostlyDbCall();
};

await this._cache.RememberAsync("BigDataCacheKey", BigDataLocalFunctionAsync, TimeSpan.FromMinutes(10));
```

### Forever

Similar to `Remember`, but your item will be cached indefinitely (see `Forget` and `Flush` for cache clearing).

```c#
this._cache.Forever("BigDataCacheKey", BigDataLocalFunction);
```

### ForeverAsync

It's `Forever`, but async:

```c#
await this._cache.ForeverAsync("BigDataCacheKey", BigDataLocalFunctionAsync);
```

### Get
`Get` will return your stored items and will return null when your item doesn't exist.

For example:
```
var storedValue = this._cache.Get<string>("BigDataCacheKey");
```

Optionally you can specify a value to return when your key doesn't exist.
```
var storedValue = this._cache.Get<string>("BigDataCacheKey", "There isn't a value");
```
### GetAsync
It's `Get`, but async:
```
var storedValue = await this._cache.GetAsync<string>("BigDataCacheKey");
```
Or
```
var storedValue = await this._cache.GetAsync<string>("BigDataCacheKey", "There isn't a value");
```

### Has

`Has` will check whether a key exists.
```
var exists = this._cache.Has("BigDataCacheKey");
```

### Flush

`Flush` will clear your entire cache.

```c#
this._cache.Flush();
```

### Forget

`Forget` will clear a specific cache entry by key.

```c#
this._cache.Forget("BigDataCacheKey");
```

## Extending Coravel With Custom Drivers

If you wish, you can create your own cache driver that extends `Coravel.Cache.Interfaces.ICache`. Maybe you want to use Coravel but use a Redis store?

First, implement a class that extends the `ICache` interface.

Next, to register your driver to be used, just pass it into the `AddCache` method:

```c#
services.AddCache(new RedisCache());

// Or, if you need the service provider to create your object:

services.AddCache(provider => new RedisCache(provider.GetService<ISomeRegisteredInterface>()));
```



