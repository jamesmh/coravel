using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Coravel.Cache.Interfaces
{
    public interface ICache
    {
        TResult Cache<TResult>(Expression<Func<TResult>> expression);
        TResult Cache<TResult>(Expression<Func<TResult>> expression, TimeSpan expiresIn);
        Task<TResult> CacheAsync<TResult>(Expression<Func<Task<TResult>>> expression);
        Task<TResult> CacheAsync<TResult>(Expression<Func<Task<TResult>>> expression, TimeSpan expiresIn);
    }
}