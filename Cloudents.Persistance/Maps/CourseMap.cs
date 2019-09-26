using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;
//using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Fluent nhibernate")]
    public sealed class CourseMap : ClassMapping<Course>
    {
        public CourseMap()
        {
            Id(x => x.Id, c => {
                c.Generator(Generators.Assigned);
                c.Column("Name");
                c.Length(150);
            });
            //Id(e => e.Id).Column("Name").GeneratedBy.Assigned().Length(150);
            Property(x => x.Count, c => c.NotNullable(true));
            //Map(x => x.Count).Not.Nullable();
            Property(x => x.Created, c => {
                c.Insert(true);
                c.Update(false);
            });
            //Map(x => x.Created).Insert().Not.Update();
            Property(x => x.State, c => {
                c.Type<NHibernate.Type.EnumStringType<ItemState>>();
            });
            //Map(x => x.State).CustomType<GenericEnumStringType<ItemState>>();
            ManyToOne(x => x.Subject, c => {
                c.Column("SubjectId");
                c.NotNullable(false);
            });
            //References(x => x.Subject).Column("SubjectId").Nullable();
            //Set<UserCourse>("_users", c=> {
            //    c.Key(k => {
            //        k.Columns(cl => cl.Name("CourseId"));
            //        k.ForeignKey("Courses_User");
            //    });
            //    c.Inverse(true);
            //    c.Cascade(Cascade.All | Cascade.DeleteOrphans);
            //    c.Table("UsersCourses");
            //    c.Access(Accessor.NoSetter);
            //}, a => a.OneToMany());

            Set(x => x.Users, c => {
                c.Key(k =>
                {
                    k.Column(cl => cl.Name("CourseId"));
                    k.ForeignKey("Courses_User");
                });
                c.Inverse(true);
                c.Cascade(Cascade.All | Cascade.DeleteOrphans);
                c.Table("UsersCourses");
                c.Access(Accessor.ReadOnly | Accessor.NoSetter);
            }, a => a.OneToMany(/*x => x.Class(typeof(UserCourse))*/)
            );

            //Property(x => x.Users, c =>
            //{
            //    c.Access(Accessor.NoSetter);
            //});

            //HasMany(x => x.Users)
            //    .KeyColumn("CourseId").ForeignKeyConstraintName("Courses_User").Inverse().AsSet();
            DynamicUpdate(true);
            //DynamicUpdate();
            OptimisticLock(OptimisticLockMode.Version);
            //OptimisticLock.Version();
            Version(x => x.Version, c => {
                c.Generated(VersionGeneration.Always);
                c.Type(new BinaryBlobType());
                c.Column(cl => {
                    cl.SqlType("timestamp");
                });
            });
            //Version(x => x.Version).CustomSqlType("rowversion").Generated.Always();
        }
    }
}
