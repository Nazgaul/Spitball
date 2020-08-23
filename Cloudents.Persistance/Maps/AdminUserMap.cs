using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class AdminUserMap : ClassMap<AdminUser>
    {
        public AdminUserMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.Email).Not.Nullable().Unique();
            Map(x => x.Country).Nullable();
            Map(e => e.SbCountry).CustomType<EnumerationType<Country>>().Nullable();
            ReadOnly();
        }
    }
}