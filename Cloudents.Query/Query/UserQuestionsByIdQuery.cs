using Cloudents.Core.DTOs;
using System.Collections.Generic;

namespace Cloudents.Query.Query
{
    public class UserDataPagingByIdQuery : IQuery<IEnumerable<QuestionFeedDto>>
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
