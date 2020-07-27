
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.DTOs
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Reflection")]
    public class CourseDto
    {
        public string Name { get;  set; }
       // public bool? IsFollowing { get;  set; }
       // public int Students { get;  set; }
       // public bool? IsTeaching { get;  set; }
    }


    public class ShortUrlDto
    {
        public ShortUrlDto(string destination)
        {
            Destination = destination;
        }

        public string Destination { get;  }
    }
}
