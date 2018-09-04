using Cloudents.Core.Entities.Db;
using FluentNHibernate.Mapping;

namespace Cloudents.Infrastructure.Database.Maps
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