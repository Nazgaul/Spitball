using System;
using System.Collections.Generic;
using System.Linq;

namespace Cloudents.Core
{
    public class PrioritySource
    {
        public string Source { get; }
        public float Priority { get; }

        public IEnumerable<string> Domains { get; }


      

        public PrioritySource(string source, float priority)

        {
            Source = source;
            Priority = priority;
        }


        public PrioritySource(string source, float priority, IEnumerable<string> domains)

        {
            Source = source;
            Priority = priority;
            Domains = domains;
        }


     
        public static readonly PrioritySource TutorWyzant = new PrioritySource("Wyzant", 1);
        //public static readonly PrioritySource TutorChegg = new PrioritySource("Chegg", 2);
        public static readonly PrioritySource TutorMe = new PrioritySource("TutorMe", 1.33f);

        public static readonly PrioritySource JobWayUp = new PrioritySource("WayUp", 1);
        public static readonly PrioritySource JobZipRecruiter = new PrioritySource("ZipRecruiter", 1);
        public static readonly PrioritySource JobJobs2Careers = new PrioritySource("Jobs2Careers", 1);
        public static readonly PrioritySource JobCareerJet = new PrioritySource("CareerJet", 3);
        public static readonly PrioritySource JobIndeed = new PrioritySource("Indeed", 5);

        public static readonly PrioritySource Unknown = new PrioritySource("Unknown", 5);


        public static readonly IReadOnlyDictionary<string, PrioritySource> DocumentPriority = new List<PrioritySource>
        {
            new PrioritySource("CourseHero" , 1,new []{ "coursehero.com/file","www.coursehero.com/subjects", "www.coursehero.com/sitemap/schools"}),
            new PrioritySource("OneClass",  3, new []{"oneclass.com/note"}),
            new PrioritySource("Cloudents",  3, new []{"spitball.co/item"}),
            new PrioritySource("StudySoup",  3 , new []{"studysoup.com/guide"}),
            new PrioritySource("Cliffsnotes",  10, new []{"cliffsnotes.com/study-guides"}),
            new PrioritySource("Koofers",  10, new []{"koofers.com/files"})
            //new PrioritySource("Studylib",  3, new []{})

        }.ToDictionary(f => f.Source, StringComparer.OrdinalIgnoreCase);


        public static readonly IReadOnlyDictionary<string, PrioritySource> FlashcardPriority = new List<PrioritySource>
        {
            new PrioritySource("StudyBlue" , 1),
            new PrioritySource("BrainScape",  1),
            new PrioritySource("CourseHero",  1),
            new PrioritySource("Quizlet",  2),
            new PrioritySource("Spitball",  4),
            new PrioritySource("Cram",  4),
            new PrioritySource("Koofers",  4),
            new PrioritySource("StudySoup",  10)
        }.ToDictionary(f => f.Source, StringComparer.OrdinalIgnoreCase);


        public static readonly IReadOnlyDictionary<string, PrioritySource> AskPriority = new List<PrioritySource>
        {

        new PrioritySource("StudyBlue", 1),
            new PrioritySource("Chegg", 1),
            new PrioritySource("britannica", 3),
            new PrioritySource("encyclopedia", 3),
            new PrioritySource("history", 3),
            new PrioritySource("khanacademy", 3),
            new PrioritySource("newworldencyclopedia", 3),
            new PrioritySource("physics", 3),
            new PrioritySource("worldatlas", 3),
            new PrioritySource("yalescientific", 3),
            new PrioritySource("boundless", 10),
            new PrioritySource("enotes", 10),
            new PrioritySource("howstuffworks", 10),
            new PrioritySource("knowledgedoor", 10),
            new PrioritySource("livescience", 10),
            new PrioritySource("lumenlearning", 10),
            new PrioritySource("quora", 10),
            new PrioritySource("reference", 10),
            new PrioritySource("socratic", 10),
            new PrioritySource("space", 10),
            new PrioritySource("thoughtco", 10),
            new PrioritySource("wikihow", 10),
            new PrioritySource("businessinsider", 10),
            new PrioritySource("snapguide", 10),
            new PrioritySource("wired", 10)

        }.ToDictionary(f => f.Source, StringComparer.OrdinalIgnoreCase);
    }
}