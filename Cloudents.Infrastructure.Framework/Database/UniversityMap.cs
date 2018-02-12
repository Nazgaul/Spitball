using Cloudents.Core.Entities.Db;
using FluentNHibernate.Mapping;

namespace Cloudents.Infrastructure.Framework.Database
{
    public class UniversityMap : ClassMap<University>
    {
        public UniversityMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();
            Map(x => x.IsDeleted);
            Schema("Zbox");
        }
    }
}