using Cloudents.Core.Entities;
//using FluentNHibernate.Mapping;
using JetBrains.Annotations;
using Cloudents.Core.Enum;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using NHibernate.Type;

namespace Cloudents.Persistence.Maps
{
    [UsedImplicitly]
    public class TutorMap : ClassMapping<Tutor>
    {
        public TutorMap()
        {
            Id(x => x.Id, c => c.Generator(Generators.Foreign<User>(f => f.Id)));
            //Id(x => x.Id).GeneratedBy.Foreign("User");
            ////CompositeId()
            ////    .KeyReference(x => x.User, "UserId");
            ////Id(x => x.UserId).GeneratedBy.Assigned();
            OneToOne(x => x.User, c=> {
                c.Constrained(true);
                c.Cascade(Cascade.None);
            });
            //HasOne(x => x.User).Constrained().Cascade.None();
            Property(x => x.Bio, c => c.Length(1000));
            //Map(x => x.Bio).Length(1000);
            Property(x => x.Price, c => c.Column(cl => cl.SqlType("smallMoney")));
            //Map(x => x.Price).CustomSqlType("smallMoney");
            Property(x => x.SellerKey);
            //Map(x => x.SellerKey);

            Bag(x => x.Reviews, c => {
                c.Key(k => {
                    k.Column("TutorId");
                    k.ForeignKey("FK_58722D65");
                });
                c.Inverse(true);
                c.Cascade(Cascade.All | Cascade.DeleteOrphans);
                c.Access(Accessor.ReadOnly | Accessor.NoSetter);
                //c.Table("TutorReview");
                c.Lazy(CollectionLazy.Lazy);
            }, a => a.OneToMany());

            //HasMany(x => x.Reviews).Access.CamelCaseField(Prefix.Underscore).Cascade.AllDeleteOrphan().Inverse();
            Bag(x => x.Calendars, c => {
                c.Cascade(Cascade.All | Cascade.DeleteOrphans);
                c.Inverse(true);
                c.Access(Accessor.ReadOnly | Accessor.NoSetter);
                c.Key(k => k.Column("TutorId"));
            }, a => a.OneToMany());
            //HasMany(x => x.Calendars).Access.CamelCaseField(Prefix.Underscore)
            //    .Cascade.AllDeleteOrphan().Inverse().AsSet();
            Set(x => x.TutorHours, c => {
                c.Inverse(true);
                c.Cascade(Cascade.All | Cascade.DeleteOrphans);
                c.Access(Accessor.ReadOnly | Accessor.NoSetter);
                c.Key(cl => cl.Column("TutorId"));
            }, a => a.OneToMany());
            //HasMany(x => x.TutorHours).Access.CamelCaseField(Prefix.Underscore)
            //    .Inverse().Cascade.All().AsSet();

            Set(x => x.StudyRooms, c => {
                c.Cascade(Cascade.All | Cascade.DeleteOrphans);
                c.Inverse(true);
                c.Access(Accessor.ReadOnly | Accessor.NoSetter);
                c.Key(cl => cl.Column("TutorId"));
                c.Table("StudyRoom");
            }, a => a.OneToMany());
            //HasMany(x => x.StudyRooms).Access.CamelCaseField(Prefix.Underscore)
            //    .Cascade.AllDeleteOrphan().Inverse().AsSet();

            Property(x => x.State, c=> c.Type<GenericEnumStringType<ItemState>>());
            //Map(x => x.State).CustomType<GenericEnumStringType<ItemState>>();
            Property(e => e.Created, c=> {
                c.Insert(true);
                c.Update(false);
            });
            //Map(e => e.Created).Insert().Not.Update();
            Property(x => x.ManualBoost, c=> {
                c.Lazy(true);
                c.NotNullable(false);
            });
            //Map(x => x.ManualBoost).LazyLoad().Nullable();
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

        public class TutorCalendarMap : ClassMapping<TutorCalendar>
        {
            public TutorCalendarMap()
            {
                Id(x => x.Id);
                //Id(x => x.Id);
                Property(x => x.Name, c=> c.NotNullable(true));
                //Map(x => x.Name).Not.Nullable();
                Property(x => x.GoogleId, c => c.NotNullable(true));
                //Map(x => x.GoogleId).Not.Nullable();
                ManyToOne(x => x.Tutor, c=> {
                    c.NotNullable(true);
                    c.Column("TutorId");
                });
                //References(x => x.Tutor).Not.Nullable();
            }
        }
    }
}