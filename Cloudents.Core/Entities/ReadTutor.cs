using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nhibernate")]
    public class ReadTutor : Entity<long>
    {
        public ReadTutor(long id, string name, string? imageName,
            IReadOnlyList<string>? allCourses,
            double? rate, int rateCount, string? bio,
            int lessons, Country sbCountry,
            Money? subscriptionPrice, string? description, ItemState state)
        {
            Id = id;
            Name = name;
            ImageName = imageName;
            Courses = allCourses?.OrderBy(o => o).Take(3);
            AllCourses = allCourses;
            Rate = rate;
            RateCount = rateCount;
            Bio = bio;
            Lessons = lessons;
            OverAllRating = (rate.GetValueOrDefault() * RateCount + 48 + Lessons * rate.GetValueOrDefault())
                            / (RateCount + 12 + Lessons);
            SubscriptionPrice = subscriptionPrice;
            Description = description;
            SbCountry = sbCountry;
            State = state;
        }

        [SuppressMessage("ReSharper", "CS8618",Justification = "Nhibernate proxy")]
        protected ReadTutor()
        {
        }


        public virtual string Name { get; protected set; }
        public virtual string? ImageName { get; protected set; }
        //public virtual IEnumerable<string>? Subjects { get; protected set; }
        //public virtual IEnumerable<string>? AllSubjects { get; protected set; }
        public virtual IEnumerable<string>? Courses { get; protected set; }
        public virtual IEnumerable<string>? AllCourses { get; protected set; }
        public virtual double? Rate { get; protected set; }
        public virtual int RateCount { get; protected set; }
        public virtual string? Bio { get; protected set; }
        public virtual int Lessons { get; protected set; }
        public virtual double OverAllRating { get; protected set; }

        public virtual ItemState State { get; set; }

        public virtual Country SbCountry { get; protected set; }

        public virtual Money? SubscriptionPrice { get;protected set; }
        public virtual string? Description { get;protected set; }

    }
}
