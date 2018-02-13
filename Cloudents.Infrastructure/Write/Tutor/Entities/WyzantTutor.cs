using Newtonsoft.Json;

namespace Cloudents.Infrastructure.Write.Tutor.Entities
{
    public class WyzantTutor
    {
        [JsonProperty("TutorID")]
        public int TutorId { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Title { get; set; }
        public int FeePerHour { get; set; }
        //public string FreeResponse { get; set; }
        //public int TravelDistance { get; set; }
        public string[] TutorPictures { get; set; }
        public string ProfileLink { get; set; }
        //public string EmailLink { get; set; }
        public Subject[] Subjects { get; set; }
        //public object Reviews { get; set; }
        //public object StarRatingAverage { get; set; }
        //public object StarRatingCount { get; set; }
       // public float TutorRank { get; set; }
        //public object College { get; set; }
        public bool OffersInPersonLessons { get; set; }
        public bool OffersOnlineLessons { get; set; }
    }

    public class Subject
    {
        public string Name { get; set; }
        //public object Description { get; set; }
    }
}
