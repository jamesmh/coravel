using Microsoft.Extensions.Logging;

namespace Coravel.Cache.Interfaces
{
    public interface ICacheOptions
    {
        ICacheOptions WithLogger(ILogger<ICache> logger);
    }
}