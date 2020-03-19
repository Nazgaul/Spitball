using System;

namespace Cloudents.Core.DTOs.Admin
{
    public class SessionDto
    {
        public DateTime Created { get; set; }
        public string Tutor { get; set; }
        public string Student { get; set; }
        public long DurationInTicks { get; set; }
        public bool ShouldSerializeDurationInTicks() => false;
        public double Duration => new TimeSpan(DurationInTicks).TotalMinutes;
    }
}
