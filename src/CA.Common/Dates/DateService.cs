using System;

namespace CA.Common.Dates
{
    public class DateService : IDateService
    {
        public DateTime GetDate()
        {
            return DateTime.Now.Date;
        }
    }
}
