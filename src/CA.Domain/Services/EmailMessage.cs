using CA.Domain.Common;
using System;

namespace CA.Domain.Services
{
    public class EmailMessage : AggregateRoot<Guid>
    {
        public string From { get; set; }

        public string To { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public DateTime? SentDate { get; set; }
    }
}