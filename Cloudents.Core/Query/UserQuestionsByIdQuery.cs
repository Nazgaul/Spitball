using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.Query
{
    public class UserQuestionsByIdQuery : IQuery<IEnumerable<QuestionFeedDto>>
    {
        public UserQuestionsByIdQuery(long id, int page)
        {
            Id = id;
            Page = page;
        }

        public long Id { get; }
        public int Page { get; }
    }
}
