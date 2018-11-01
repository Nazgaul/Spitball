using JetBrains.Annotations;
using System.Collections.Generic;
using Cloudents.Core.Enum;

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
        public IList<QuestionDto> Result { get; set; }
        public IEnumerable<QuestionFilter> FacetState { get; set; }
        public IEnumerable<QuestionSubject> FacetSubject { get; set; }
    }
}