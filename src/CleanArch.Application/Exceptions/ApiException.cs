using System;
using System.Globalization;

namespace CleanArch.Application.Exceptions
{
    public class ApiException : ApplicationException
    {
        public ApiException() : base() { }

        public ApiException(string message) : base(message) { }

        public ApiException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
