namespace Cloudents.Core.DTOs
{
    public class DocumentSeoDto
    {
        public string Country { get; set; }
        public string Name { get; set; }
        public string BoxName { get; set; }
        public string Description { get; set; }

        public string Discriminator { get; set; }

        public string ImageUrl { get; set; }

        public string DepartmentName { get; set; }
    }
}
