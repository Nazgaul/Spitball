using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities.Db;

namespace Cloudents.Infrastructure.Data.Maps
{
    [SuppressMessage("ReSharper", "UnusedMember.Global",Justification = "Fluent nhibernate")]
    public class HiLoGeneratorMap :SpitballClassMap<HiLoGenerator>
    {
        public HiLoGeneratorMap()
        {
            Id(x => x.Id).GeneratedBy.Native();
            Map(x => x.TableName).Not.Nullable();
            Map(x => x.NextHi).Not.Nullable();
        }
    }
}