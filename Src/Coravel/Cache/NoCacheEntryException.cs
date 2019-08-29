using System;

namespace Coravel.Cache
{
    public class NoCacheEntryException : Exception
    {
        public NoCacheEntryException(string message) : base(message)
        {
            
        }
    }
}