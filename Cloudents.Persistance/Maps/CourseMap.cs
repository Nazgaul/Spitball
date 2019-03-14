using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistance.Maps
{
    [SuppressMessage("ReSharper", "UnusedMember.Global",Justification = "Fluent nhibernate")]
    public sealed class CourseMap : ClassMap<Course>
    {
        public CourseMap()
        {
            Id(e => e.Name).GeneratedBy.Assigned().Length(150);
            Map(x => x.Count).Not.Nullable();
            Map(x => x.Created);
            //HasMany(x => x.Users)
            //   .KeyColumn("Id")
            //   .LazyLoad()
            //   .Inverse()
            //   .ForeignKeyCascadeOnDelete();

            HasManyToMany(x => x.Users)
                .ParentKeyColumn("CourseId")
                .ChildKeyColumn("UserId")
                .ForeignKeyConstraintNames("Courses_User", "User_Courses")
             .Table("UsersCourses").AsSet();

            HasMany(x => x.Documents)
                .KeyColumn("CourseName")
                .LazyLoad()
                .Inverse()
                .ForeignKeyCascadeOnDelete();


            //HasMany(x => x.Questions)
            //    .KeyColumn("CourseId")
            //    .LazyLoad()
            //    .Inverse()
            //    .ForeignKeyCascadeOnDelete();

            HasMany(x => x.Questions).Cascade.None();
            // HasMany(x => x.Users).Cascade.None();
            Map(x => x.State);
            SchemaAction.Update();
        }
    }
}
