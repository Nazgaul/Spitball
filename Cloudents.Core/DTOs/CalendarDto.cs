using System;

namespace Cloudents.Core.DTOs
{
    public class CalendarDto
    {
        public CalendarDto(string id, string name, bool isShared)
        {
            Id = id;
            Name = name;
            IsShared = isShared;
        }

        public string Id { get; }
        public string Name { get; }
        public bool IsShared { get; set; }
    }

    public class GoogleAppointmentDto
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}