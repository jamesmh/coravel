using System;

namespace Coravel.Cache.Database.Core
{
    internal sealed class CacheEntryDBModel
    {
        public string? Key { get; set; }
        public string? Value { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }

        internal bool IsExpired()
        {
            return ExpiresAt.UtcDateTime <= DateTimeOffset.UtcNow;
        }
    }
}
