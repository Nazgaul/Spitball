using JetBrains.Annotations;
using System.Collections.Generic;

namespace Cloudents.Core.DTOs
{
    //public class ResultWithFacetDto<T>
    //{

    //    [ItemCanBeNull,CanBeNull]
    //    public IEnumerable<T> Result { get; set; }
    //    public IEnumerable<string> Facet { get; set; }
    //}

    public class ResultWithTotalCount<T>
    {
        public IEnumerable<T> Result { get; set; }

        public int Count { get; set; }
    }

    //public class ResultWithFacetDto2<T>
    //{
    //    public ResultWithFacetDto2()
    //    {
    //        Result = new List<T>();
    //    }
    //    [ItemCanBeNull]
    //    public List<T> Result { get; set; }
    //    public IEnumerable<string> Facet { get; set; }
    //}
}