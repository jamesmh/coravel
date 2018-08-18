using System;

namespace Coravel.Mail.Exceptions
{
    public class NoMailRendererFound : Exception
    {
        public NoMailRendererFound(string message) : base(message) {
            
        }
    }
}