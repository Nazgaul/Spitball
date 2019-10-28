using System.Collections.Generic;
using Cloudents.Core.DTOs;

namespace Cloudents.Query.Query
{
    public class UserDataPagingByIdQuery : IQuery<IEnumerable<QuestionFeedDto>>, IQuery<IEnumerable<DocumentFeedDto>>
    {
        public UserDataPagingByIdQuery(long id, int page)
        {
            Id = id;
            Page = page;
        }

        public long Id { get; }
        public int Page { get; }
    }


    public class UserAnswersByIdQuery : UserDataPagingByIdQuery
    {
        public UserAnswersByIdQuery(long id, int page) : base(id, page)
        {

        }
       
    }
}
