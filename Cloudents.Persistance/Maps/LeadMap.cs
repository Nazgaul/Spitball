using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class LeadMap :ClassMap<Lead>
    {
        public LeadMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            References(x => x.Course).Not.Nullable();
            References(x => x.User).Nullable();
            References(x => x.University).Nullable();
            Map(x => x.Phone);
            Map(x => x.Text);
            Map(x => x.UtmSource);
        }
        
    }
}