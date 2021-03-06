﻿using System;
using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs.Users
{
    public abstract class UserPurchaseDto
    {
        public abstract ContentType Type { get;  }
       
        public DateTime Date { get; set; }
    }

    public class PurchasedDocumentDto : UserPurchaseDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Course { get; set; }
        public string Preview { get; set; }
        public string Url { get; set; }
        public decimal Price { get; set; }

        [NonSerialized] public ContentType type;

        public override ContentType Type => type;
    }

    public class PurchasedSessionDto : UserPurchaseDto
    {
        [NonSerialized]
        public decimal _oldPrice;
        [NonSerialized]
        public double? _price;

        public string TutorName { get; set; }
        public double TotalMinutes => Duration.GetValueOrDefault().TotalMinutes;

        public TimeSpan? Duration { get; set; }

        public bool ShouldSerializeDuration() => false;

        public string? TutorImage { get; set; }
        public long TutorId { get; set; }
        public override ContentType Type => ContentType.TutoringSession;
        public double Price => _price ?? (double) _oldPrice;
    }

    public class PurchaseCourseDto : UserPurchaseDto
    {
        public long Id { get; set; }

        public double Price { get; set; }
        public override ContentType Type => ContentType.Course;



        public string Preview { get; set; }
        public string Name { get; set; }

        [NonSerialized] public int version;
    }

    public class PurchasedBuyPointsDto : UserPurchaseDto
    {
        private decimal _price;
        public Guid Id { get; set; }

        public string Country { get; set; }

        public decimal Price

        {
            get
            {
                Country country = Entities.Country.FromCountry(Country);
                return (_price, country.Name) switch
                {
                    (500, Entities.Country.UsStr) => 6M,
                    (1000,Entities.Country.UsStr) => 10M,
                    (100, Entities.Country.UsStr) => 1.5M,
                    (1500, Entities.Country.IsraelStr) => 60M,
                    (750, Entities.Country.IsraelStr) => 30M,
                    (250, Entities.Country.IsraelStr) => 10M,
                    (_, Entities.Country.IndiaStr) => _price,
                    (_, _) => _price * country.ConversationRate
                };
            }
            set => _price = value;
        }

        public override ContentType Type => ContentType.BuyPoints;
    }
}
