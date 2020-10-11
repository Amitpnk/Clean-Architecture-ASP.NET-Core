using System;

namespace CA.CrossCuttingConcerns.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message)
            : base(message)
        {
        }
    }
}
