using System;
using System.Threading.Tasks;

namespace Coravel.Cache.Interfaces;

/// <summary>
/// An interface that defines methods for caching data.
/// </summary>
public interface ICache
{
    /// <summary>
    /// Retrieves an item from the cache using the specified key. If the item does not exist, it executes the cacheFunc and stores the result in the cache for the specified duration.
    /// </summary>
    /// <typeparam name="TResult">The type of the item to retrieve.</typeparam>
    /// <param name="key">The key of the item to retrieve.</param>
    /// <param name="cacheFunc">The function to execute if the item does not exist in the cache.</param>
    /// <param name="expiresIn">The duration for which the item should be cached.</param>
    /// <returns>The cached item or the result of the cacheFunc.</returns>
    TResult Remember<TResult>(string key, Func<TResult> cacheFunc, TimeSpan expiresIn);

    /// <summary>
    /// Retrieves an item from the cache using the specified key asynchronously. If the item does not exist, it executes the cacheFunc and stores the result in the cache for the specified duration.
    /// </summary>
    /// <typeparam name="TResult">The type of the item to retrieve.</typeparam>
    /// <param name="key">The key of the item to retrieve.</param>
    /// <param name="cacheFunc">The function to execute if the item does not exist in the cache.</param>
    /// <param name="expiresIn">The duration for which the item should be cached.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the cached item or the result of the cacheFunc.</returns>
    Task<TResult> RememberAsync<TResult>(string key, Func<Task<TResult>> cacheFunc, TimeSpan expiresIn);

    /// <summary>
    /// Retrieves an item from the cache using the specified key. If the item does not exist, it executes the cacheFunc and stores the result in the cache indefinitely.
    /// </summary>
    /// <typeparam name="TResult">The type of the item to retrieve.</typeparam>
    /// <param name="key">The key of the item to retrieve.</param>
    /// <param name="cacheFunc">The function to execute if the item does not exist in the cache.</param>
    /// <returns>The cached item or the result of the cacheFunc.</returns>
    TResult Forever<TResult>(string key, Func<TResult> cacheFunc);

    /// <summary>
    /// Retrieves an item from the cache using the specified key asynchronously. If the item does not exist, it executes the cacheFunc and stores the result in the cache indefinitely.
    /// </summary>
    /// <typeparam name="TResult">The type of the item to retrieve.</typeparam>
    /// <param name="key">The key of the item to retrieve.</param>
    /// <param name="cacheFunc">The function to execute if the item does not exist in the cache.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the cached item or the result of the cacheFunc.</returns>
    Task<TResult> ForeverAsync<TResult>(string key, Func<Task<TResult>> cacheFunc);

    /// <summary>
    /// Checks if an item exists in the cache using the specified key asynchronously.
    /// </summary>
    /// <param name="key">The key of the item to check.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the item exists or not.</returns>
    Task<bool> HasAsync(string key);

    /// <summary>
    /// Retrieves an item from the cache using the specified key asynchronously.
    /// </summary>
    /// <typeparam name="TResult">The type of the item to retrieve.</typeparam>
    /// <param name="key">The key of the item to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the cached item or null if it does not exist.</returns>
    Task<TResult> GetAsync<TResult>(string key);

    /// <summary>
    /// Removes all items from the cache.
    /// </summary>
    void Flush();

    /// <summary>
    /// Removes an item from the cache using the specified key.
    /// </summary>
    /// <param name = "key" > The key of the item to remove.</param >
    void Forget(string key);
}