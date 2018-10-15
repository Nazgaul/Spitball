using Cloudents.Core.Enum;
using JetBrains.Annotations;
using System.Collections.Generic;

namespace Cloudents.Core.DTOs
{
    public class ResultWithFacetDto<T>
    {
        [ItemCanBeNull]
        public IEnumerable<T> Result { get; set; }
        public IEnumerable<string> Facet { get; set; }
    }


    public class QuestionWithFacetDto
    {
        //public ResultWithFacetDto2()
        //{
        //    Facets = new Dictionary<string, IEnumerable<string>>();
        //}
        [ItemCanBeNull]
        public IEnumerable<QuestionDto> Result { get; set; }
        public IEnumerable<QuestionFilter> FacetState { get; set; }
        public IEnumerable<KeyValuePair<int, string>> FacetSubject { get; set; }
}
}