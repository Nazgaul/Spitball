using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Cloudents.Core.Extension;

namespace Cloudents.Core.DTOs
{
    public class TutorCardDto
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public IEnumerable<string> Courses { get; set; }
        public IEnumerable<string> Subjects { get; set; }

        public decimal TutorPrice { get; set; }
        public string TutorCountry { get; set; }

        [NonSerialized] 
        public bool NeedSerializer;


        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Json return")] 
        public string Price => TutorPrice.ToString("C0", CultureInfo.CurrentUICulture.ChangeCultureBaseOnCountry(TutorCountry));

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by json.net")]
        public bool ShouldSerializeTutorPrice()
        {
            return NeedSerializer;
        }
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by json.net")]
        public bool ShouldSerializeTutorCountry()
        {
            return NeedSerializer;
        }


        public double? Rate { get; set; }
        public int ReviewsCount { get; set; }

        public string Bio { get; set; }

        public string University { get; set; }

        public int Lessons { get; set; }

        private sealed class UserIdEqualityComparer : IEqualityComparer<TutorCardDto>
        {
            public bool Equals(TutorCardDto x, TutorCardDto y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.UserId == y.UserId;
            }

            public int GetHashCode(TutorCardDto obj)
            {
                return obj.UserId.GetHashCode();
            }
        }

        public static IEqualityComparer<TutorCardDto> UserIdComparer { get; } = new UserIdEqualityComparer();
    }
}
