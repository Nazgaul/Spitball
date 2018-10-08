using System.Collections.Generic;
using Cloudents.Core.Enum;
using JetBrains.Annotations;

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
        public IEnumerable<string> FacetSubject { get; set; }
    }
}