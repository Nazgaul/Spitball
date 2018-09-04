using System.Collections.Generic;

namespace Cloudents.Web.Models
{
    //public class WebResponseWithFacet<T>
    //{
    //    public IEnumerable<T> Result { get; set; }
    //    public IEnumerable<string> Facet { get; set; }

    //    public string NextPageLink { get; set; }
    //}

    public class WebResponseWithFacet<T>
    {
        public IEnumerable<T> Result { get; set; }
        // public Dictionary<string, IEnumerable<string>> Filters { get; set; }

        //public  Dictionary<>
        public IEnumerable< Filters> Filters { get; set; }

        public IEnumerable<string> Sort { get; set; }


        public string NextPageLink { get; set; }
    }
    

    public class Filters
    {
        public Filters(string id, string title, IEnumerable<string> data)
        {
            Id = id;
            Title = title;
            Data = data;
        }

        public string Id { get; set; }
        public string Title { get; set; }

        public IEnumerable<string> Data { get; set; }
    }



}