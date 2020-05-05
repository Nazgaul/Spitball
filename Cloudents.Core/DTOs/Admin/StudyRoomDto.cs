using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cloudents.Core.DTOs.Admin
{
    public class StudyRoomDto
    {
        [NonSerialized]
        [SuppressMessage("ReSharper", "UnassignedField.Global", Justification = "Query over nhibernate")]
        public TimeSpan? DurationT;


       

        public Guid SessionId { get; set; }
        public string TutorName { get; set; }
        public string UserName { get; set; }
        public DateTime Created { get; set; }

        public string Duration
        {
            get
            {
                if (DurationT.HasValue)
                {
                    return DurationT.Value.TotalMinutes.ToString(CultureInfo.InvariantCulture);
                }

                return "OnGoing";
            }
        }

        
        public long TutorId { get; set; }
        public long UserId { get; set; }
    }
}
