﻿using Cloudents.Core.DTOs.Feed;
using Cloudents.Core.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs.Tutors
{
    public class TutorCardDto : FeedDto
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public string? Image { get; set; }
        public IEnumerable<string> Courses { get; set; }
        public IEnumerable<string> Subjects { get; set; }

        //public decimal TutorPrice { get; set; }
        public override FeedType Type => FeedType.Tutor;
        public Country Country { get; set; }


        [NonSerialized]
        public bool NeedSerializer;



        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Json return")]
        public decimal Price { get; set; }

        public string Currency => Country.RegionInfo.ISOCurrencySymbol;

        public decimal? DiscountPrice { get; set; }
      
       
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by json.net")]
        public bool ShouldSerializeTutorCountry()
        {
            return NeedSerializer;
        }


        public double? Rate { get; set; }
        public int ReviewsCount { get; set; }

        public string Bio { get; set; }


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
