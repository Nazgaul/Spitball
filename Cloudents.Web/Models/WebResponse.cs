using System.Collections;
using System.Collections.Generic;

namespace Cloudents.Web.Models
{
    public class WebResponseWithFacet<T>
    {
        public IEnumerable<T> Result { get; set; }

        public IEnumerable<string> Filters { get; set; }

        public long? Count { get; set; }
        public string NextPageLink { get; set; }
    }

 

    //public interface IFilters
    //{
    //    string Id { get; }
    //    string Title { get; }

    //    IEnumerable Data { get; }
    //}


    //public class Filters<T> : IFilters
    //{
    //    public Filters(string id, string title, IEnumerable<KeyValuePair<T, string>> data)
    //    {
    //        Id = id;
    //        Title = title;
    //        Data2 = data;
    //    }

    //    public string Id { get; set; }
    //    public string Title { get; set; }
    //    public IEnumerable Data => Data2;


    //    private IEnumerable<KeyValuePair<T, string>> Data2 { get; set; }
    //}



}