//using System;
//using System.Collections.Generic;
//using System.Text;
//using Cloudents.Core.Entities;
//using FluentNHibernate.Mapping;

//namespace Cloudents.Persistence.Maps
//{
//    public class CourseEnrollmentMap : ClassMap<CourseEnrollment>
//    {
//        public CourseEnrollmentMap()
//        {
//            Id(x => x.Id).GeneratedBy.Guid();
//            References(x => x.Course).Not.Nullable();
//            References(x => x.User).Not.Nullable();

//        }
//    }
//}
