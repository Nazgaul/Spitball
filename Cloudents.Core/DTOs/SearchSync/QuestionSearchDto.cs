using System.Collections.Generic;

namespace Cloudents.Core.DTOs.SearchSync
{

    public class SearchWrapperDto <T>
    {
        public IEnumerable<T> Update { get; set; }
        public IEnumerable<string> Delete { get; set; }
        public long Version { get; set; }
    }

    //public class QuestionSearchDto
    //{

    //    public long Id { get; set; } //key readonly

    //    public DateTime? DateTime { get; set; } //readonly

    //    public string Text { get; set; } //search readonly


    //    public string[] Prefix
    //    {
    //        get
    //        {
    //            var arr = new List<string>();
    //            //if (Subject.HasValue)
    //            //{
    //            //    if (System.Enum.IsDefined(typeof(QuestionSubject), Subject.Value))
    //            //    {
    //            //        arr.AddRange(Subject.GetEnumLocalizationAllValues());
    //            //    }
    //            //}

    //            if (!string.IsNullOrEmpty(Text))
    //            {
    //                arr.Add(Text);
    //            }

    //            if (arr.Count == 0)
    //            {
    //                return null;
    //            }

    //            return arr.ToArray();
    //        }
    //    } //search


    //    public string Country { get; set; }
       

    //    public string Language { get; set; }

    //    public string Course { get; set; }


    //    //public QuestionSubject? Subject { get; set; } // facetable readonly
    //    public QuestionFilter? State { get; set; }
    //    public string UniversityName { get; set; }
    //    public string[] Tags { get; set; }
    //}
}