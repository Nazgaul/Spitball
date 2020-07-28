using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using FluentNHibernate.Mapping;
using NHibernate;

namespace Cloudents.Persistence.Maps
{
    public class StudyRoomMap : ClassMap<StudyRoom>
    {
        public StudyRoomMap()
        {
            DynamicUpdate();
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.Identifier).Not.Nullable().Unique();
            References(x => x.Tutor).Not.Nullable();

            Map(x => x.TopologyType).Column("Type");
           
            Component(x => x.DateTime, z => {
                z.Map(m => m.CreationTime).Column("DateTime");
                z.Map(m => m.UpdateTime).Column("Updated");
            });
            Map(x => x.OnlineDocumentUrl).Not.Nullable();
            HasMany(x => x.Sessions).Access.CamelCaseField(Prefix.Underscore)
                .KeyColumn("StudyRoomId")
                .Inverse().Cascade.AllDeleteOrphan();


            //TODO do not extra lazy load on set
            HasMany(x => x.Users).Access.CamelCaseField(Prefix.Underscore)
                .Inverse().Cascade.AllDeleteOrphan().AsSet();

            Map(x => x.OldPrice).Column("Price").CustomType(nameof(NHibernateUtil.Currency));

            Map(x => x.Price)
                .CustomType<MoneyCompositeUserType>();
           
            //Map(x => x.StudyRoomType).CustomType<GenericEnumStringType<StudyRoomType>>();


            HasMany(x => x.ChatRooms).Inverse().Cascade.AllDeleteOrphan();//.Inverse();
            HasMany(x => x.StudyRoomPayments).Access.CamelCaseField(Prefix.Underscore)
                .Inverse().Cascade.AllDeleteOrphan();

            //HasMany(x => x.UserTokens)
            //    .Access.CamelCaseField(Prefix.Underscore)
            //    .Cascade.AllDeleteOrphan();

            
            DiscriminateSubClassesOnColumn("StudyRoomType");//,StudyRoomType.Private.ToString())
                //.CustomType<GenericEnumStringType<StudyRoomType>>().Not.Nullable();

        }

    }

    public class PrivateStudyRoomMap : SubclassMap<PrivateStudyRoom>
    {
        public PrivateStudyRoomMap()
        {
            Map(x => x.Name).Length(500);
            DiscriminatorValue(StudyRoomType.Private.ToString());
        }
    }

    public class BroadCastStudyRoomMap : SubclassMap<BroadCastStudyRoom>
    {
        public BroadCastStudyRoomMap()
        {
            DynamicUpdate();
            DiscriminatorValue(StudyRoomType.Broadcast.ToString());
            Map(x => x.BroadcastTime);
            Map(x => x.Description).Length(4000).Nullable();
            //Cannot put not nullable because no inverse on course.
            References(x => x.Course).Column("CourseId")/*.Not.Nullable()*/.ForeignKey("StudyRoom_course2");

            Component(x => x.Schedule, z =>
            {
                z.Map(x => x!.End).Column("ScheduleEnd");
                z.Map(x => x!.CronString).Column("ScheduleCron");
                z.Map(x => x!.Start).Column("ScheduleStart");
            });
        }
    }

    public class StudyRoomUserMap : ClassMap<StudyRoomUser>
    {
        public StudyRoomUserMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            References(x => x.User).Not.Nullable();
            References(x => x.Room).Column("StudyRoomId").Not.Nullable();
           // Map(x => x.Online).Not.Nullable();
        }
    }


    //public class DiscriminatorValueConvention  :ISubclassConvention
    //{
    //    public void Apply(ISubclassInstance instance)
    //    {
    //        instance.DiscriminatorValue(instance.EntityType.Name);
    //    }
    //}
}