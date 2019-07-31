using Cloudents.Core.Enum;

namespace Cloudents.Web.Models
{
    public class QuestionsRequest : VerticalRequest
    {

        public QuestionFilter?[] Filter { get; set; }
    }
}