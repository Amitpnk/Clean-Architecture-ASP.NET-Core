using System;
using System.Runtime.Serialization;

namespace CA.CrossCuttingConcerns.Exceptions
{

    [Serializable]
    public class NotFoundException : Exception
    {
        public NotFoundException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not found.")
        {
        }

        protected NotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
        {
        }
    }
}
