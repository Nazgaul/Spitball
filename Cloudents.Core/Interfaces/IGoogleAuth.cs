using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using JetBrains.Annotations;

namespace Cloudents.Core.Interfaces
{
    public interface IGoogleAuth
    {
        [ItemCanBeNull]
        Task<ExternalAuthDto> LogInAsync(string token, CancellationToken cancellationToken);
    }


    public interface IGoogleDocument
    {
        Task<string> CreateOnlineDocAsync(string documentName, CancellationToken token);
    }

    public interface ICalendarService
    {
        Task SaveTokenAsync(string token, long userId, string baseUrl, CancellationToken cancellationToken);

        Task<CalendarEventDto> ReadCalendarEventsAsync(long userId,IEnumerable<string> calendars, DateTime @from,
            DateTime to, CancellationToken cancellationToken);
        Task BookCalendarEventAsync(User tutor, User student, DateTime from, DateTime to, CancellationToken cancellationToken);
        Task DeleteDeclinedEventCalendarAsync(CancellationToken cancellationToken);
        Task<IEnumerable<CalendarDto>> GetUserCalendarsAsync(long userId, CancellationToken cancellationToken);
    }


}