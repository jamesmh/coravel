using System;

namespace Coravel.Mailer.Mail.Exceptions
{
    public class NoMailRendererFound : Exception
    {
        public NoMailRendererFound(string message) : base(message)
        {

        }
    }
}