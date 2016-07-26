namespace Zbang.Zbox.ViewModel.Dto
{
    public class HomePageDataDto
    {
        public HomePageDataDto()
        {
            HomePageUniversityData = new HomePageUniversityData();
        }
        public HomePageStats HomePageStats { get; set; }
        public HomePageUniversityData HomePageUniversityData { get; set; }
        //public IEnumerable<RecommendBoxDto> Boxes { get; set; }
    }

    public class HomePageStats
    {
        public int DocumentCount { get; set; }
        public int StudentsCount { get; set; }
        public int BoxesCount { get; set; }
    }

    public class HomePageUniversityData
    {
        public HomePageUniversityData()
        {
            SignupColor = "#EC610D";
            BackgroundImage = "DefaultUni.png";
            HeaderBackgroundColor = "rgba(0,0,0,.9)";
            VideoBackgroundColor = "#FFF";
            VideoFontColor = "#383838";
            Logo = "/images/site/logo_spitball.png";
        }
        public string HeaderBackgroundColor { get; set; }
        public string BackgroundImage { get; set; }
        public string UniversityName { get; set; }
        public string VideoBackgroundColor { get; set; }
        public string VideoFontColor { get; set; }
        public string SignupColor { get; set; }

        public string Logo { get; set; }
    }

}
