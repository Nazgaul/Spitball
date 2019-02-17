using Cloudents.Core.Enum;

namespace Cloudents.Web.Models
{
    public class QuestionsRequest : VerticalRequest
    {
      //  [DisplayFormat(HtmlEncode = true)]
        //public string Term { get; set; }
       // public int? Page { get; set; }

        public QuestionSubject[] Source { get; set; }

        public QuestionFilter?[] Filter { get; set; }
    }
}