﻿using System;

namespace Cloudents.Core.Entities
{
    public class CourseEnrollment : Entity<Guid>
    {
        public CourseEnrollment(User user, Course course)
        {
            User = user;
            Course = course;
        }

        public virtual User User { get; set; }
        public virtual Course Course { get; set; }
        public virtual string? Receipt { get; protected set; }

        public virtual Money? Price { get; set; }

        //Coupon
        //recepit
    }
}