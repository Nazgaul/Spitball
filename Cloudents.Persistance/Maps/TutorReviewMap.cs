using Cloudents.Core.Entities;
//using FluentNHibernate.Mapping;
using JetBrains.Annotations;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using NHibernate.Type;

namespace Cloudents.Persistence.Maps
{
    [UsedImplicitly]
    public sealed class TutorReviewMap : ClassMapping<TutorReview>
    {
        public TutorReviewMap()
        {
            Id(x => x.Id, c => c.Generator(Generators.GuidComb));
            //Id(x => x.Id).GeneratedBy.GuidComb();
            ManyToOne(x => x.User, c =>
            {
                c.NotNullable(true);
                c.Column("UserId");
            });
            //References(x => x.User).Not.Nullable().Column("UserId");
            ManyToOne(x => x.Tutor, c =>
            {
                c.ForeignKey("FK_58722D65");
                c.NotNullable(true);
                c.Column(cl => {
                    cl.SqlType("bigint");
                    cl.Name("TutorId");
                });
            });
            //References(x => x.Tutor).Not.Nullable().Column("TutorId");
            ////References(x => x.Room).Not.Nullable().Column("RoomId");
            Property(x => x.DateTime, c => c.NotNullable(true));
            //Map(x => x.DateTime).Not.Nullable();
            Property(x => x.Review, c => c.Length(1000));
            //Map(x => x.Review).Length(1000);
            Property(x => x.Rate, c => c.NotNullable(true));
            //Map(x => x.Rate).Not.Nullable();
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