namespace Cloudents.Core.DTOs
{
    public class ExternalAuthDto
    {
        public string Name { get; set; }
        public string Email { get; set; }

        public string Id { get; set; }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Email)}: {Email}, {nameof(Id)}: {Id}";
        }
    }
}