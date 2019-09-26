using Cloudents.Core.Entities;
//using FluentNHibernate.Mapping;
using JetBrains.Annotations;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using NHibernate.Type;

namespace Cloudents.Persistence.Maps
{
    [UsedImplicitly]
    public sealed class UserCourseMap : ClassMapping<UserCourse>
    {
        public UserCourseMap()
        {
            ComposedId(x => {

                x.ManyToOne(p => p.User, c =>  c.Column("UserId"));
                x.ManyToOne(p => p.Course, c => c.Column("CourseId"));
            });
            //CompositeId()
            //    .KeyReference(x => x.User, "UserId")
            //    .KeyReference(x => x.Course, "CourseId");
            Property(x => x.CanTeach, c => c.NotNullable(true));
            //Map(e => e.CanTeach).Not.Nullable();
            Table("UsersCourses");
            //Table("UsersCourses");
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