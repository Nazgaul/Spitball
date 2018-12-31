﻿using System.Collections.Generic;
using Cloudents.Core.DTOs.Admin;

namespace Cloudents.Query.Query.Admin
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