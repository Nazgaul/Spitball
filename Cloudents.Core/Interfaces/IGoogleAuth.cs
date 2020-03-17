using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IGoogleAuth
    {
        Task<ExternalAuthDto?> LogInAsync(string token, CancellationToken cancellationToken);
    }

    public interface IGoogleAnalytics
    {
        Task TrackEventAsync(string category, string action, string label, CancellationToken token);
    }


    public interface IGoogleDocument
    {
        Task<string> CreateOnlineDocAsync(string documentName, CancellationToken token);
    }

    public interface ICalendarService
    {
        Task SaveTokenAsync(string token, long userId, string baseUrl, CancellationToken cancellationToken);

        Task<IEnumerable<GoogleAppointmentDto>> ReadCalendarEventsAsync(long userId, IEnumerable<string> calendars, DateTime @from,
            DateTime to, CancellationToken cancellationToken);
        Task BookCalendarEventAsync(User tutor, User student, GoogleTokens googleTokens, DateTime from, DateTime to, CancellationToken cancellationToken);
        Task DeleteDeclinedEventCalendarAsync(CancellationToken cancellationToken);
        Task<IEnumerable<CalendarDto>> GetUserCalendarsAsync(long userId, CancellationToken cancellationToken);
    }


}