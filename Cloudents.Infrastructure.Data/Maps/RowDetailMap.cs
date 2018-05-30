using Cloudents.Core.Entities.Db;
using FluentNHibernate.Mapping;

namespace Cloudents.Infrastructure.Data.Maps
{
    public class RowDetailMap : ComponentMap<RowDetail>
    {
        public RowDetailMap()
        {
            Map(m => m.CreatedUser).Insert().Not.Update();
            Map(m => m.CreationTime).Insert().Not.Update();
            Map(m => m.UpdateTime).Insert();
            Map(m => m.UpdatedUser).Insert();
        }
    }
}