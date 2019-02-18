using Cloudents.Core.Enum;

namespace Cloudents.Web.Models
{
    public class QuestionsRequest : VerticalRequest
    {
        public QuestionSubject[] Source { get; set; }

        public QuestionFilter?[] Filter { get; set; }
    }
}