namespace Cloudents.Query.HomePage
{
    public class StatsDto
    {
        //public StatsDto(int documents, int tutors, int students, float reviews)
        //{
        //    Documents = documents.ToString("N0");
        //    Tutors = tutors.ToString("N0");
        //    Students = students.ToString("N0");
        //    Reviews = reviews;
        //}

        public int Documents { get; set; }
        public int Tutors { get; set; }
        public int Students { get; set; }

        //public string ReviewsPercentage => Reviews.ToString("P0");
        public float Reviews { get; set; }
    }
}