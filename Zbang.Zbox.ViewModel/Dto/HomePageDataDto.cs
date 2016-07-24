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
            H1Text = "#f5f5f5";
            SignupColor = "#EC610D";
            BackgroundImage = "/Images/HomePage2/HP_image.jpg";
            HeaderBackgroundColor = "#181c1d";
            VideoBackgroundColor = "#FFF";
            VideoFontColor = "#181c1d";
        }
        public string HeaderBackgroundColor { get; set; }
        public string BackgroundImage { get; set; }
        public string H1Text { get; set; }
        public string VideoBackgroundColor { get; set; }
        public string VideoFontColor { get; set; }
        public string SignupColor { get; set; }
    }

}
