using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto.BoxDtos
{
    public class BoxSeoDto
    {
        public string Name { get; set; }
        public BoxType BoxType { get; set; }

        public string Country { get; set; }
        public string UniversityName { get; set; }

        public string DepartmentName { get; set; }
    }
}
