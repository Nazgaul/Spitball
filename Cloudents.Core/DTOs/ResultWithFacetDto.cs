using System.Collections.Generic;
using JetBrains.Annotations;

namespace Cloudents.Core.DTOs
{
    //public class ResultWithFacetDto<T>
    //{
       
    //    [ItemCanBeNull,CanBeNull]
    //    public IEnumerable<T> Result { get; set; }
    //    public IEnumerable<string> Facet { get; set; }
    //}


    public class ResultWithFacetDto2<T>
    {
        public ResultWithFacetDto2()
        {
            Result = new List<T>();
        }
        [ItemCanBeNull]
        public List<T> Result { get; set; }
        public IEnumerable<string> Facet { get; set; }
    }
}