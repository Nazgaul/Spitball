using Cloudents.Core.Entities.Db;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Database.Maps
{
    [UsedImplicitly]
    public sealed class UniversityMap : SpitballClassMap<University>
    {
        public UniversityMap()
        {
            Id(x => x.Id).GeneratedBy.HiLo($"sb.{nameof(HiLoGenerator)}", nameof(HiLoGenerator.NextHi), "10", $"{nameof(HiLoGenerator.TableName)}='{nameof(University)}'");
                //.GeneratedBy.Assigned();
            Map(x => x.IsDeleted);
            Map(x => x.Name).Column("UniversityName");
            Map(x => x.Extra);
            Map(x => x.ExtraSearch);
            Map(x => x.Country).Not.Nullable().Length(2);
            Map(x => x.Pending);
            Component(x => x.RowDetail);

            Schema("Zbox");

            
        }
    }
}