﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nhibernate")]
    public class ReadTutor : Entity<long>
    {
        public ReadTutor(long id, string name, string image, string imageName, 
            IReadOnlyList<string>? allSubjects,
            IReadOnlyList<string>? allCourses,
            decimal price, double? rate, int rateCount, string bio,
            int lessons, Country sbCountry,
            decimal? subsidizedPrice, Money? subscriptionPrice, string? description)
        {
            Id = id;
            Name = name;
            Image = image;
            ImageName = imageName;
            Subjects = allSubjects?.OrderBy(o => o).Take(3);
            AllSubjects = allSubjects;
            Courses = allCourses?.OrderBy(o => o).Take(3);
            AllCourses = allCourses;
            Price = price;
            Rate = rate;
            RateCount = rateCount;
            Bio = bio;
            Lessons = lessons;
            //case 115
            OverAllRating = (rate.GetValueOrDefault() * RateCount + 48 + Lessons * rate.GetValueOrDefault())
                            / (RateCount + 12 + Lessons);
            SubsidizedPrice = subsidizedPrice;
            SubscriptionPrice = subscriptionPrice;
            Description = description;
            SbCountry = sbCountry;
        }

        [SuppressMessage("ReSharper", "CS8618",Justification = "Nhibernate proxy")]
        protected ReadTutor()
        {
        }


        public virtual string Name { get; protected set; }
        public virtual string Image { get; protected set; }
        public virtual string ImageName { get; protected set; }
        public virtual IEnumerable<string>? Subjects { get; protected set; }
        public virtual IEnumerable<string>? AllSubjects { get; protected set; }
        public virtual IEnumerable<string>? Courses { get; protected set; }
        public virtual IEnumerable<string>? AllCourses { get; protected set; }
        public virtual decimal Price { get; protected set; }
        public virtual double? Rate { get; protected set; }
        public virtual int RateCount { get; protected set; }
        public virtual string Bio { get; protected set; }
        public virtual int Lessons { get; protected set; }
        public virtual double OverAllRating { get; protected set; }

      
        public virtual decimal? SubsidizedPrice { get; protected set; }

        public virtual Country SbCountry { get; protected set; }

        public virtual Money? SubscriptionPrice { get;protected set; }
        public virtual string? Description { get;protected set; }

    }
}
