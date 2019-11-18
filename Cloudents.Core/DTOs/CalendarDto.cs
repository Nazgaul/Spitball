using System;

namespace Cloudents.Core.DTOs
{
    public class CalendarDto
    {
        public CalendarDto(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; }
        public string Name { get; }
    }

    public class GoogleAppointmentDto
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}