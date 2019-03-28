using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    [SuppressMessage("ReSharper", "UnusedMember.Global",Justification = "Fluent nhibernate")]
    public sealed class CourseMap : ClassMap<Course>
    {
        public CourseMap()
        {
            Id(e => e.Id).Column("Name").GeneratedBy.Assigned().Length(150);
            Map(x => x.Count).Not.Nullable();
            Map(x => x.Created);
            //HasMany(x => x.Users)
            //   .KeyColumn("Id")
            //   .LazyLoad()
            //   .Inverse()
            //   .ForeignKeyCascadeOnDelete();

            //HasManyToMany(x => x.Users)
            //    .ParentKeyColumn("CourseId")
            //    .ChildKeyColumn("UserId")
            //    .ForeignKeyConstraintNames("Courses_User", "User_Courses")
            //    //.Inverse()
            // .Table("UsersCourses").AsSet();

            //HasMany(x => x.Documents)
            //    .KeyColumn("CourseName")
            //    .LazyLoad()
            //    .Inverse()
            //    .Cascade.None();


            //HasMany(x => x.Questions)
            //    .KeyColumn("CourseId")
            //    .LazyLoad()
            //    .Inverse()
            //    .Cascade.None();

            //HasMany(x => x.Tutors)
            //    .Table("TutorsCourses")
            //    .KeyColumn("CourseId")
            //    //.Cascade.Delete()
            //    .LazyLoad()
            //    .Inverse().Cascade.AllDeleteOrphan()
            //    .AsList();

            HasMany(x => x.Users)
                .KeyColumn("CourseId").ForeignKeyConstraintName("Courses_User").Inverse().AsSet();

            // HasMany(x => x.Questions).Cascade.None();
            // HasMany(x => x.Users).Cascade.None();
            Map(x => x.State);
        }
    }
}
