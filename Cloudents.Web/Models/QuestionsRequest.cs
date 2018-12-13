using Cloudents.Common;
using Cloudents.Core.Enum;

namespace Cloudents.Web.Models
{
    public class QuestionsRequest : IPaging
    {
      //  [DisplayFormat(HtmlEncode = true)]
        public string Term { get; set; }
        public int? Page { get; set; }

        public QuestionSubject[] Source { get; set; }

        public QuestionFilter?[] Filter { get; set; }

    }
}