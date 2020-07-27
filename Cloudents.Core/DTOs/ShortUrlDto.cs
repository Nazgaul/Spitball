namespace Cloudents.Core.DTOs
{
    public class ShortUrlDto
    {
        public ShortUrlDto(string destination)
        {
            Destination = destination;
        }

        public string Destination { get;  }
    }
}