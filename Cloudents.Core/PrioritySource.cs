using System;
using System.Collections.Generic;
using System.Linq;

namespace Cloudents.Core
{
    public class PrioritySource
    {
        public string Source { get; }
        public int Priority { get; }

        public IEnumerable<string> Domains { get; }

        protected PrioritySource()
        {
            
        }

        public PrioritySource(string source, int priority)
        {
            Source = source;
            Priority = priority;
        }

        private PrioritySource(string source, int priority, IEnumerable<string> domains)
        {
            Source = source;
            Priority = priority;
            Domains = domains;
        }

        public static readonly PrioritySource TutorWyzant = new PrioritySource("Wyzant", 1);
        //public static readonly PrioritySource TutorChegg = new PrioritySource("Chegg", 2);
        public static readonly PrioritySource TutorMe = new PrioritySource("TutorMe", 3);

        public static readonly PrioritySource JobWayUp = new PrioritySource("WayUp", 1);
        public static readonly PrioritySource JobZipRecruiter = new PrioritySource("ZipRecruiter", 2);
        public static readonly PrioritySource JobJobs2Careers = new PrioritySource("Jobs2Careers", 3);
        public static readonly PrioritySource JobCareerJet = new PrioritySource("CareerJet", 4);
        public static readonly PrioritySource JobIndeed = new PrioritySource("Indeed", 5);

        public static readonly PrioritySource Unknown = new PrioritySource("Unknown", 5);

        public static readonly IReadOnlyDictionary<string, PrioritySource> DocumentPriority = new List<PrioritySource>
        {
            new PrioritySource("CourseHero" , 1,new []{ "coursehero.com/file","www.coursehero.com/subjects", "www.coursehero.com/sitemap/schools"}),
            new PrioritySource("OneClass",  2, new []{"oneclass.com/note"}),
            new PrioritySource("Cloudents",  2, new []{"spitball.co/item"}),
            new PrioritySource("StudySoup",  2 , new []{"studysoup.com/guide"}),
            new PrioritySource("Cliffsnotes",  3, new []{"cliffsnotes.com/study-guides"}),
            new PrioritySource("Koofers",  3, new []{"koofers.com/files"})
        }.ToDictionary(f => f.Source, StringComparer.OrdinalIgnoreCase);


        public static readonly IReadOnlyDictionary<string, PrioritySource> FlashcardPriority = new List<PrioritySource>
        {
            new PrioritySource("StudyBlue" , 1, new []{"www.studyblue.com/notes/note"}),
           // new PrioritySource("BrainScape",  2),
            new PrioritySource("CourseHero",  3,new []{"coursehero.com/flashcards"}),
            new PrioritySource("Quizlet",  3, new []{"quizlet.com"}),
            new PrioritySource("Spitball",  4,new [] {"spitball.co/flashcard"}),
            new PrioritySource("Cram",  5, new []{"cram.com/flashcards"}),
            new PrioritySource("Koofers",  5,new []{"koofers.com/flashcards"}),
            new PrioritySource("StudySoup",  5,new []{"studysoup.com/flashcard"})
        }.ToDictionary(f => f.Source, StringComparer.OrdinalIgnoreCase);


        public static readonly IReadOnlyDictionary<string, PrioritySource> AskPriority = new List<PrioritySource>
        {
            new PrioritySource("StudyBlue", 1,new []{"studyblue.com/notes/hh"}),
            new PrioritySource("Chegg", 2,new []{"www.chegg.com/homework-help/questions-and-answers"}),
            new PrioritySource("britannica", 3,new [] {"britannica.com"}),
            new PrioritySource("encyclopedia", 3,new []{"encyclopedia.com"}),
            new PrioritySource("history", 3,new []{"history.com"}),
            new PrioritySource("khanacademy", 3, new []{"khanacademy.org"}),
            new PrioritySource("newworldencyclopedia", 3,new []{"newworldencyclopedia.org"}),
            new PrioritySource("physics", 3,new []{"physics.org"}),
            new PrioritySource("worldatlas", 3,new [] {"worldatlas.com"}),
            new PrioritySource("yalescientific", 3,new []{"yalescientific.org"}),
            new PrioritySource("boundless", 4,new [] {"boundless.com"}),
            new PrioritySource("enotes", 4,new []{"enotes.com"}),
            new PrioritySource("howstuffworks", 4,new [] {"howstuffworks.com"}),
            new PrioritySource("knowledgedoor", 4,new []{"knowledgedoor.com"}),
            new PrioritySource("livescience", 4,new []{"livescience.com"}),
            new PrioritySource("lumenlearning", 4,new []{"lumenlearning.com"}),
            new PrioritySource("quora", 4,new[]{"quora.com"}),
            new PrioritySource("reference", 4,new []{"reference.com"}),
            new PrioritySource("socratic", 4,new []{"socratic.org"}),
            new PrioritySource("space", 4,new[] {"space.com"}),
            new PrioritySource("thoughtco", 4,new []{"thoughtco.com"}),
            new PrioritySource("wikihow", 4, new [] {"wikihow.com"}),
            new PrioritySource("businessinsider", 5,new []{"businessinsider.com"}),
            new PrioritySource("snapguide", 5,new [] {"snapguide.com"}),
            new PrioritySource("wired", 5,new []{"wired.com"})
        }.ToDictionary(f => f.Source, StringComparer.OrdinalIgnoreCase);
    }
}