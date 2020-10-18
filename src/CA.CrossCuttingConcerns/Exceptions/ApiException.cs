using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace CA.CrossCuttingConcerns.Exceptions
{
    [Serializable]
    public class ApiException : Exception
    {
        public ApiException() : base() { }

        public ApiException(string message) : base(message) { }

        public ApiException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }

        protected ApiException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }
    }
}
