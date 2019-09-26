using Cloudents.Core.Entities;
//using FluentNHibernate.Mapping;
using JetBrains.Annotations;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;

namespace Cloudents.Persistence.Maps
{
    [UsedImplicitly]
    public class StudyRoomMap : ClassMapping<StudyRoom>
    {
        public StudyRoomMap()
        {
            Id(x => x.Id, c => c.Generator(Generators.GuidComb));
            //Id(x => x.Id).GeneratedBy.GuidComb();
            Property(x => x.Identifier, c => {
                c.NotNullable(true);
                c.Unique(true);
            });
            //Map(x => x.Identifier).Not.Nullable().Unique();
            ManyToOne(x => x.Tutor, c => {
                c.NotNullable(true);
                c.Column("TutorId");
            });
            //References(x => x.Tutor).Not.Nullable();

            Component(x => x.Type);
            //Map(x => x.Type);
            Property(x => x.DateTime, c => c.NotNullable(true));
            //Map(x => x.DateTime).Not.Nullable();
            Property(x => x.OnlineDocumentUrl, c => c.NotNullable(true));
            //Map(x => x.OnlineDocumentUrl).Not.Nullable();

            Bag<StudyRoomSession>("_sessions", c => {
                c.Inverse(true);
                c.Cascade(Cascade.All | Cascade.DeleteOrphans);
                c.Key(k => k.Column("StudyRoomId"));
            }, a => a.OneToMany());
            //HasMany(x => x.Sessions).Access.CamelCaseField(Prefix.Underscore)
            //    .KeyColumn("StudyRoomId")
            //    .Inverse().Cascade.AllDeleteOrphan();

            Bag(x => x.Users, c => {
                c.Inverse(true);
                c.Cascade(Cascade.All | Cascade.DeleteOrphans);
                c.Access(Accessor.ReadOnly | Accessor.NoSetter);
                c.Key(k => k.Column("StudyRoomId"));
            }, a => a.OneToMany());
            //HasMany(x => x.Users).Access.CamelCaseField(Prefix.Underscore)
            //    .Inverse().Cascade.AllDeleteOrphan();


        }

    }

    public class StudyRoomUserMap : ClassMapping<StudyRoomUser>
    {
        public StudyRoomUserMap()
        {
            Id(x => x.Id, c => c.Generator(Generators.GuidComb));
            //Id(x => x.Id).GeneratedBy.GuidComb();
            ManyToOne(x => x.User, c => {
                c.NotNullable(true);
                c.Column("UserId");
            });
            //References(x => x.User).Not.Nullable();
            ManyToOne(x => x.Room, c => {
                c.Column("StudyRoomId");
                c.NotNullable(true);
            });
            //References(x => x.Room).Column("StudyRoomId").Not.Nullable();
            Property(x => x.Online, c => c.NotNullable(true));
            //Map(x => x.Online).Not.Nullable();
        }
    }
}