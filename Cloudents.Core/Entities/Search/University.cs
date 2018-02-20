using Cloudents.Core.Interfaces;
using Microsoft.Spatial;

namespace Cloudents.Core.Entities.Search
{
    public class University : ISearchObject
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string[] Extra { get; set; }

        public string Image { get; set; }

        public GeographyPoint GeographyPoint { get; set; }
    }
}