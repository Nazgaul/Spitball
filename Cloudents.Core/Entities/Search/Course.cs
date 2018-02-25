
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Entities.Search
{
   // [SerializePropertyNamesAsCamelCase]
    public class Course : ISearchObject
    {
      //  [Key]
        public string Id { get; set; }

       // [IsSearchable]
        public string Name { get; set; }

        public string Prefix { get; set; }


        //  [IsSearchable]

        public string Code { get; set; }

      //  [IsFilterable]
        public long UniversityId { get; set; }
    }
}