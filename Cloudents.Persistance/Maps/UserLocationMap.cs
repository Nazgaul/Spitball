using Cloudents.Core.Entities;
//using FluentNHibernate.Mapping;
using JetBrains.Annotations;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;

namespace Cloudents.Persistence.Maps
{
    [UsedImplicitly]
    public class UserLocationMap : ClassMapping<UserLocation>
    {
        public UserLocationMap()
        {
            Map();
        }

        private void Map()
        {
            Id(x => x.Id, c => c.Generator(Generators.GuidComb));
            //Id(x => x.Id).GeneratedBy.GuidComb();
            ManyToOne(x => x.User, c =>
            {
                c.NotNullable(true);
                c.Column("UserId");
                c.ForeignKey("UserLocation_User");
            });
            //References(x => x.User).Column("UserId").ForeignKey("UserLocation_User");
            Component(x => x.TimeStamp);
            //Component(x => x.TimeStamp);
            Property(x => x.Ip, c => c.NotNullable(false));
            //Map(x => x.Ip).Nullable();
            Property(x => x.Country, c =>
            {
                c.Length(10);
                c.NotNullable(false);
            });
            //Map(x => x.Country).Length(10).Nullable();
            Property(x => x.FingerPrint, c => c.NotNullable(false));
            //Map(x => x.FingerPrint).Nullable();
            Property(x => x.UserAgent, c => c.NotNullable(false));
            //Map(x => x.UserAgent).Nullable();
            SchemaAction(NHibernate.Mapping.ByCode.SchemaAction.Update);
            //SchemaAction.Update();
        }
    }
}