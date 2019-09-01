using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.DTOs
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Dto")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Dto")]
    public class CalendarEventDto
    {
        public CalendarEventDto(DateTime @from, DateTime to)
        {
            From = @from;
            To = to;
        }

        public DateTime From { get;  }
        public DateTime To { get;  }
    }
}