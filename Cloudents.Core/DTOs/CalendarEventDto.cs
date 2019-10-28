using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.DTOs
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Dto")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Dto")]
    public class CalendarEventDto
    {
        public CalendarEventDto( IEnumerable<DateTime> busySlot)
        {
            BusySlot = busySlot;
        }


        public IEnumerable<DateTime> BusySlot { get;  }

    }
}