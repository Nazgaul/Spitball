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
                .Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore)
                .ChildKeyColumn("UserId")
                .Inverse()
                .ForeignKeyConstraintNames("Courses_User", "User_Courses")
             .Table("UsersCourses").AsSet();

            HasMany(x => x.Documents)
                .KeyColumn("CourseName")
                .Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore)
                .LazyLoad()
                .Inverse()
                .ForeignKeyCascadeOnDelete();

            HasMany(x => x.Questions)
                .ReadOnly()
                .Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore)
                .Cascade.None();
            // HasMany(x => x.Users).Cascade.None();
            Map(x => x.State);
            SchemaAction.Update();
        }
    }
}
