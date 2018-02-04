using System;

using NHibernate.Id;

namespace Zbang.Zbox.Infrastructure.IdGenerator
{
    public class GuidIdGenerator : IGuidIdGenerator
    {
        public static Guid GetGuid()
        {
            var combGenerator = new GuidCombGenerator();
            return (Guid)combGenerator.Generate(null, null);
        }

        public Guid GetId()
        {
            return GetGuid();
        }
    }
}
