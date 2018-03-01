using Cloudents.Core.Entities.Db;
using FluentNHibernate.Mapping;
using NHibernate.Mapping;

namespace Cloudents.Infrastructure.Framework.Database.Maps
{
    public class MailGunStudentMap : ClassMap<MailGunStudent>
    {
        public MailGunStudentMap()
        {
            Id(x => x.Id).GeneratedBy.Native();
            Map(x => x.Sent).Length(8000);
            Map(x => x.ShouldSend);
            Table("students2");
        }
    }
}