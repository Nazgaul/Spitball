using System.Collections.Generic;
using Cloudents.Application.DTOs.Admin;
using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Query.Admin
{
    public class AdminPageQuery : IQuery<IEnumerable<QuestionWithoutCorrectAnswerDto>>
    {
        public AdminPageQuery(int page)
        {
            Page = page;

        }

        public int Page { get; }
    }
}
