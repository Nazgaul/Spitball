using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using Microsoft.Spatial;

namespace Cloudents.Core.Entities.Search
{
    public class University : ISearchObject
    {
        public string Id { get; set; }
        public string Name { [UsedImplicitly] get; set; }

        public string Extra { [UsedImplicitly] get; set; }
        public string Prefix { [UsedImplicitly] get; set; }

        public string Image { [UsedImplicitly] get; set; }

        public GeographyPoint GeographyPoint { [UsedImplicitly] get; set; }
    }
}