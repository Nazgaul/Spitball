using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto.BoxDtos
{
    public class BoxSeoDto
    {
        public string Name { get; set; }
        public string CourseId { get; set; }
        public string Professor { get; set; }
        public string Url { get; set; }
        public BoxType BoxType { get; set; }

        public string Country { get; set; }
        public string UniversityName { get; set; }
    }
}
