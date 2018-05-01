using Cloudents.Core.Entities.Db;
using FluentNHibernate.Mapping;

namespace Cloudents.Infrastructure.Database.Maps
{
    public class UniversityMap : ClassMap<University>
    {
        public UniversityMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();
            Map(x => x.IsDeleted);
            //TODO : this appear twice - need to fix that
            Map(x => x.Name).Column("UniversityName");
            Map(x => x.Extra);
            Map(x => x.ExtraSearch);
            Schema("Zbox");
        }
    }
}