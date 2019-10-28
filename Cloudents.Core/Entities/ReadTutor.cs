﻿using System.Collections.Generic;
using System.Linq;

namespace Cloudents.Core.Entities
{
    public class ReadTutor : Entity<long>
    {
        public ReadTutor(long id, string name, string image, IEnumerable<string> allSubjects, IEnumerable<string> allCourses,
            decimal price, double? rate, int rateCount, string bio, string university, int lessons, string country, decimal? subsidizedPrice)
        {
            Id = id;
            Name = name;
            Image = image;
            Subjects = allSubjects?.OrderBy(o => o).Take(3);
            AllSubjects = allSubjects;
            Courses = allCourses?.OrderBy(o => o).Take(3);
            AllCourses = allCourses;
            Price = price;
            Rate = rate;
            RateCount = rateCount;
            Bio = bio;
            University = university;
            Lessons = lessons;
            Country = country;
            OverAllRating = (rate.GetValueOrDefault() * RateCount + 48 + Lessons * rate.GetValueOrDefault()) 
                            / (RateCount + 12 + Lessons);
            SubsidizedPrice = subsidizedPrice;
        }

        protected ReadTutor()
        {
            
        }


        public virtual string Name { get; protected set; }
        public virtual string Image { get; protected set; }
        public virtual IEnumerable<string> Subjects { get; protected set; }
        public virtual IEnumerable<string> AllSubjects { get; protected set; }
        public virtual IEnumerable<string> Courses { get; protected set; }
        public virtual IEnumerable<string> AllCourses { get; protected set; }
        public virtual decimal Price { get; protected set; }
        public virtual double? Rate { get; protected set; }
        public virtual int RateCount { get; protected set; }
        public virtual string Bio { get; protected set; }
        public virtual string University { get; protected set; }
        public virtual int Lessons { get; protected set; }
        public virtual double OverAllRating { get; protected set; }

        public virtual string  Country { get; protected set; }
        public virtual decimal? SubsidizedPrice{ get; protected set; }
        
    }
}
