using JetBrains.Annotations;
using System.Collections.Generic;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    public class ResultWithFacetDto<T>
    {
       
        [ItemCanBeNull,CanBeNull]
        public IEnumerable<T> Result { get; set; }
        public IEnumerable<string> Facet { get; set; }
    }


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


    public class QuestionWithFacetDto
    {
        public QuestionWithFacetDto()
        {
            Result = new List<QuestionFeedDto>();
        }
        [ItemCanBeNull]
        public IList<QuestionFeedDto> Result { get; set; }
        public IEnumerable<QuestionFilter> FacetState { get; set; }
        public IEnumerable<QuestionSubject> FacetSubject { get; set; }
    }
}