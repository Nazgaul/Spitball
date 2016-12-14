namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class FileSeo
    {
        public string Country { get; set; }
        public string Name { get; set; }
        public string BoxName { get; set; }
        public string Description { get; set; }

        public string Url { get; set; }

        public string Discriminator { get; set; }

        public string ImageUrl { get; set; }


        public string DepartmentName { get; set; }

    }

    public class FlashcardSeoDto
    {
        public string UniversityName { get; set; }
        public long BoxId { get; set; }
        public string BoxName { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
    }
}
