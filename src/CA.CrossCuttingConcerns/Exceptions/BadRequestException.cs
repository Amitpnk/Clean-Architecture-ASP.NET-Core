using System;
using System.Runtime.Serialization;

namespace CA.CrossCuttingConcerns.Exceptions
{
    [Serializable]
    public class BadRequestException : Exception
    {
        public BadRequestException(string message)
            : base(message)
        {
        }

        protected BadRequestException(SerializationInfo info, StreamingContext context)
           : base(info, context)
        {
        }

    }
}
