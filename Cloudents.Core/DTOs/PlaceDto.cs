using Cloudents.Core.Models;

namespace Cloudents.Core.DTOs
{
    public class PlaceDto
    {
        public string Name { get; set; }
        public double Rating { get; set; }
        public string Address { get; set; }
        public bool Open { get; set; }

        public GeoPoint Location { get; set; }

        public string Image { get; set; }
        public string PlaceId { get; set; }

        public bool Hooked { get; set; }
    }

    public class HookedDto : System.IEquatable<HookedDto>
    {
        public string Id { get; set; }

        public bool Equals(HookedDto other)
        {
            return this.Id == other.Id;
        }
    }
}
