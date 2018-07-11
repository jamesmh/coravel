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

await this._cache.Remember("BigDataCacheKey", BigDataLocalFunctionAsync, TimeSpan.FromMinutes(10));
```

### Forever

Similar to `Remember`, but your item will be cached indefinitely (see `Forget` and `Flush` for cache clearing).

```c#
this._cache.Forever("BigDataCacheKey", BigDataLocalFunction);
```

### ForeverAsync

It's `Forever`, but async:

```c#
await this._cache.Forever("BigDataCacheKey", BigDataLocalFunctionAsync);
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



