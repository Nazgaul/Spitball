using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Cloudents.Core.Query
{
    public class QuestionSubjectQuery : IQuery<IEnumerable<QuestionSubjectDto>>
    {
        public QuestionSubjectQuery(/*CultureInfo cultureInfo*/)
        {
            CultureInfo = Thread.CurrentThread.CurrentUICulture;
        }

        public CultureInfo CultureInfo { get; }
    }



}