
namespace Zbang.Zbox.ViewModel.Dto.Dashboard
{
    public class UniversityDashboardInfoDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Cover { get; set; }

        public int NumOfBoxes { get; set; }
        public int NumOfUsers { get; set; }
        public int NumOfItems { get; set; }

        public string Logo { get; set; }


        public string Url { get; set; }
    }
}
