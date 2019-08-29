using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Coravel.Cache.Interfaces
{
    public interface ICache
    {
        TResult Remember<TResult>(string key, Func<TResult> cacheFunc, TimeSpan expiresIn);
        Task<TResult> RememberAsync<TResult>(string key, Func<Task<TResult>> cacheFunc, TimeSpan expiresIn);

        TResult Forever<TResult>(string key, Func<TResult> cacheFunc);
        Task<TResult> ForeverAsync<TResult>(string key, Func<Task<TResult>> cacheFunc);

        Task<bool> HasAsync(string key);

        Task<TResult> GetAsync<TResult>(string key);

        void Flush();

        void Forget(string key);
    }
}