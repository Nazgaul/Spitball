using System.Collections.Generic;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Query.Admin
{
    public class AdminPageQuery : IQuery<IEnumerable<QuestionWithoutCorrectAnswerDto>>
    {
        public AdminPageQuery(int _page)
        {
            page = _page;

        }

        public int page { get; set; }
    }
}
