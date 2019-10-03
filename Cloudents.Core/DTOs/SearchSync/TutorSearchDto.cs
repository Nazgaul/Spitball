using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs.SearchSync
{
    public class TutorSearchDto
    {
        public long UserId { get; set; } 
        public string Name { get; set; } 
        public string Bio { get; set; } 
        public string Image { get; set; } 
        public double Price { get; set; } 
        public double Rate { get; set; }

        public override string ToString()
        {
            return $"{nameof(UserId)}: {UserId}";
        }

        public int ReviewsCount { get; set; } 
        public byte[] Version { get; set; } 
        public IEnumerable<string> Courses { get; set; } 
        public IEnumerable<string> Subjects { get; set; }
        public string Country { get; set; }
        [SuppressMessage("ReSharper", "InconsistentNaming",Justification = "Database value")]
        public long SYS_CHANGE_VERSION { get; set; }

        public string University { get; set; }
        public int LessonsCount { get; set; }
        public long Id { get; set; }
        [SuppressMessage("ReSharper", "InconsistentNaming",Justification = "Database value")] 
        public string SYS_CHANGE_OPERATION { get; set; }
        public ItemState State { get; set; }
        public long VersionAsLong => BitConverter.ToInt64(Version.Reverse().ToArray(), 0);
    }
}