using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;

namespace Cloudents.Core.Interfaces
{
    public interface ICalendarService
    {
        Task SaveTokenAsync(string token, long userId, string baseUrl, CancellationToken cancellationToken);

        Task<IEnumerable<GoogleAppointmentDto>> ReadCalendarEventsAsync(long userId, IEnumerable<string> calendars, DateTime @from,
            DateTime to, CancellationToken cancellationToken);
        Task DeleteDeclinedEventCalendarAsync(CancellationToken cancellationToken);
        Task<IEnumerable<CalendarDto>> GetUserCalendarsAsync(long userId, CancellationToken cancellationToken);

        Task SendCalendarInviteAsync(IEnumerable<string> emails, DateTime from, DateTime to,
            string title, string? description, CancellationToken cancellationToken);
        string GetEmailFromToken(string token);
    }
}