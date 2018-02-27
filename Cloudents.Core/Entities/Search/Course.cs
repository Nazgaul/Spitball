
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Entities.Search
{
    public class Course : ISearchObject
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string[] Prefix { get; set; }

        public string Code { get; set; }

        public long UniversityId { get; set; }
    }
}