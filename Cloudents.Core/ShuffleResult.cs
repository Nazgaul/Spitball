using System.Collections.Generic;
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
                o.PrioritySource.Priority * o.Order).ThenBy(n => n.PrioritySource.Priority);
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
        private string Source { get; }
        public int Priority { get; }

        public PrioritySource(string source, int priority)
        {
            Source = source;
            Priority = priority;
        }

        public override string ToString()
        {
            return Source;
        }

        public static readonly PrioritySource TutorWyzant = new PrioritySource("Wyzant", 1);
        public static readonly PrioritySource TutorChegg = new PrioritySource("Chegg", 2);
        public static readonly PrioritySource TutorMe = new PrioritySource("TutorMe", 3);


        public static readonly PrioritySource JobWayUp = new PrioritySource("WayUp", 1);
        public static readonly PrioritySource JobZipRecruiter = new PrioritySource("ZipRecruiter", 2);
        public static readonly PrioritySource JobJobs2Careers = new PrioritySource("Jobs2Careers", 3);
        public static readonly PrioritySource JobCareerJet = new PrioritySource("CareerJet", 4);
        public static readonly PrioritySource JobIndeed = new PrioritySource("Indeed", 5);
    }
}
