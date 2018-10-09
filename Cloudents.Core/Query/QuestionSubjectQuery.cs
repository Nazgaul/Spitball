using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Query
{
    public class QuestionSubjectQuery : IQuery<IEnumerable<QuestionSubjectDto>>
    {
        public QuestionSubjectQuery(/*CultureInfo cultureInfo*/)
        {
            CultureInfo = Thread.CurrentThread.CurrentUICulture;
        }

        public CultureInfo CultureInfo { get; private set; }
    }


    
}