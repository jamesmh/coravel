using System;

namespace Coravel.Cache.Database.Core
{
    internal class CacheEntryDBModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
    }
}
