using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using Cloudents.Core.Extension;

namespace Cloudents.Core
{
    public interface IShuffleable
    {
        PrioritySource PrioritySource { get; }
        int Order { get; set; }
    }

    public interface IShuffle
    {
        IEnumerable<T> DoShuffle<T>(IEnumerable<T> result) where T : IShuffleable;
    }

    public class Shuffle : IShuffle
    {
        public IEnumerable<T> DoShuffle<T>(IEnumerable<T> result) where T : IShuffleable
        {
            return result.OrderBy(o =>
                o.PrioritySource.Priority * o.Order).ThenBy(n => n.PrioritySource.Priority).ThenBy(p => p.Order);
            //if (result == null)
            //{
            //    return null;
            //}
            //var listOfResult = result.ToList();
            //if (listOfResult.Count == 0)
            //{
            //    return listOfResult;
            //}
            //var objectToKeep = listOfResult.RemoveAndGet(0);

            //var list = new List<T> { objectToKeep };

            //while (listOfResult.Count > 0)
            //{
            //    var lastElement = list.Last();
            //    var index = listOfResult.FindIndex(f => !Equals(f.Bucket, lastElement.Bucket));
            //    if (index == -1)
            //    {
            //        index = 0;
            //    }
            //    list.Add(listOfResult.RemoveAndGet(index));
            //}

            //return list;
        }
    }



    public class PrioritySource
    {
        public string Source { get; }
        public int Priority { get; }

        public IEnumerable<string> Domains { get; }

        public PrioritySource(string source, int priority)
        {
            Source = source;
            Priority = priority;
        }

        public PrioritySource(string source, int priority, IEnumerable<string> domains)
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
            //new PrioritySource("Studylib",  3, new []{})
        }.ToDictionary(f => f.Source, StringComparer.OrdinalIgnoreCase);


        public static readonly IReadOnlyDictionary<string, PrioritySource> FlashcardPriority = new List<PrioritySource>
        {
            new PrioritySource("StudyBlue" , 1),
            new PrioritySource("BrainScape",  2),
            new PrioritySource("CourseHero",  3),
            new PrioritySource("Quizlet",  3),
            new PrioritySource("Spitball",  4),
            new PrioritySource("Cram",  5),
            new PrioritySource("Koofers",  5),
            new PrioritySource("StudySoup",  5)
        }.ToDictionary(f => f.Source, StringComparer.OrdinalIgnoreCase);


        public static readonly IReadOnlyDictionary<string, PrioritySource> AskPriority = new List<PrioritySource>
        {
            new PrioritySource("StudyBlue", 1),
            new PrioritySource("Chegg", 2),
            new PrioritySource("britannica", 3),
            new PrioritySource("encyclopedia", 3),
            new PrioritySource("history", 3),
            new PrioritySource("khanacademy", 3),
            new PrioritySource("newworldencyclopedia", 3),
            new PrioritySource("physics", 3),
            new PrioritySource("worldatlas", 3),
            new PrioritySource("yalescientific", 3),
            new PrioritySource("boundless", 4),
            new PrioritySource("enotes", 4),
            new PrioritySource("howstuffworks", 4),
            new PrioritySource("knowledgedoor", 4),
            new PrioritySource("livescience", 4),
            new PrioritySource("lumenlearning", 4),
            new PrioritySource("quora", 4),
            new PrioritySource("reference", 4),
            new PrioritySource("socratic", 4),
            new PrioritySource("space", 4),
            new PrioritySource("thoughtco", 4),
            new PrioritySource("wikihow", 4),
            new PrioritySource("businessinsider", 5),
            new PrioritySource("snapguide", 5),
            new PrioritySource("wired", 5)
        }.ToDictionary(f => f.Source, StringComparer.OrdinalIgnoreCase);

        //public static readonly IReadOnlyDictionary<string, int> DocumentPriority = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        //{
        //    {"CourseHero" , 1},
        //    {"OneClass",  2},
        //    {"Spitball",  2},
        //    {"StudySoup",  2},
        //    {"Cliffsnotes",  3},
        //    {"Koofers",  3},
        //    {"Studylib",  3},
        //};

        //public static readonly PrioritySource DocumentCourseHero = new PrioritySource(1);
        //public static readonly PrioritySource DocumentOneClass = new PrioritySource(2);
        //public static readonly PrioritySource DocumentSpitball = new PrioritySource(2);
        //public static readonly PrioritySource DocumentStudySoup = new PrioritySource(2);
        //public static readonly PrioritySource DocumentCliffsNotes = new PrioritySource(3);
        //public static readonly PrioritySource DocumentKoofers = new PrioritySource(3);
        //public static readonly PrioritySource DocumentStudyLib = new PrioritySource(3);

    }
}
