namespace Zbang.Zbox.ViewModel.Dto.Search
{
    public class SearchBoxes
    {
        public SearchBoxes()
        {

        }

        public SearchBoxes(long id, string name, string image, string professor, string courseCode, string url)
        {
            Id = id;
            Name = name;
            Image = image;
            Professor = professor;
            CourseCode = courseCode;
            Url = url;
        }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Professor { get; set; }
        public string CourseCode { get; set; }
        public long Id { get; set; }

        public string Url { get; set; }
    }
}
