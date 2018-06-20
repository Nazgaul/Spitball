using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Cloudents.Core
{
    public class PrioritySource
    {
        public string Source { get; }
        public float Priority { get; }

        public IEnumerable<string> Domains { get; }


        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "We need that for serialization")]
        protected PrioritySource()
        {

        }

        public PrioritySource(string source, float priority)

        {
            Source = source;
            Priority = priority;
        }


        private PrioritySource(string source, float priority, IEnumerable<string> domains)

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
            new PrioritySource(CloudentsSource,  3, new []{"spitball.co/item"}),
            new PrioritySource("StudySoup",  3 , new []{"studysoup.com/guide"}),
            new PrioritySource("Koofers",  10, new []{"koofers.com/files"})

        }.ToDictionary(f => f.Source, StringComparer.OrdinalIgnoreCase);

        public const string CloudentsSource = "Cloudents";

        public static readonly IReadOnlyDictionary<string, PrioritySource> FlashcardPriority = new List<PrioritySource>
        {
            new PrioritySource("StudyBlue" , 1,new []{"www.studyblue.com/notes/note"}),
            new PrioritySource("brainscape",  1,new []{"brainscape.com/flashcards"}),
            new PrioritySource("CourseHero",  1,new []{"coursehero.com/flashcards"}),
            new PrioritySource("Quizlet",  2,new []{"quizlet.com"}),
            new PrioritySource(CloudentsSource,  4,new []{"spitball.co/flashcard"}),
            new PrioritySource("Cram",  4,new [] {"cram.com/flashcards"}),
            new PrioritySource("Koofers",  4,new [] {"koofers.com/flashcards"}),
            new PrioritySource("StudySoup",  10,new [] {"studysoup.com/flashcard"})
        }.ToDictionary(f => f.Source, StringComparer.OrdinalIgnoreCase);
       
    }
}