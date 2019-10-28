﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;

namespace Cloudents.Core.DTOs
{
    public class TutorCardDto : FeedDto
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public IEnumerable<string> Courses { get; set; }
        public IEnumerable<string> Subjects { get; set; }

        public decimal TutorPrice { get; set; }
        public override FeedType Type => FeedType.Tutor;
        public string TutorCountry
        {
            get => _tutorCountry;
            set
            {
                _mergeCultureInfo = CultureInfo.CurrentUICulture.ChangeCultureBaseOnCountry(value);
                _tutorCountry = value;
            }
        }

        [NonSerialized] private CultureInfo _mergeCultureInfo;

        [NonSerialized] 
        public bool NeedSerializer;

        private string _tutorCountry;


        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Json return")] 
        public string Price => TutorPrice.ToString("C0", _mergeCultureInfo);

        public string DiscountPrice
        {
            get
            {
                if (_tutorCountry.Equals("IN", StringComparison.OrdinalIgnoreCase))
                {
                    return 0.ToString("C0", _mergeCultureInfo);
                }

                return null;
            }
        }

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
                if (x is null) return false;
                if (y is null) return false;
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
