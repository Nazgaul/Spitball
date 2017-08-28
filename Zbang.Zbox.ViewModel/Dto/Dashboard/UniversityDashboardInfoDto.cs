
namespace Zbang.Zbox.ViewModel.Dto.Dashboard
{
    public class UniversityDashboardInfoDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Cover { get; set; }

        public int Boxes { get; set; }
        public int Users { get; set; }
        public int Items { get; set; }

        public string BtnColor { get; set; }
        public string StripColor { get; set; }
        public string BtnFontColor { get; set; }

        public string Logo { get; set; }

        public string Url { get; set; }
    }
}
