using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Enum;

namespace Cloudents.Web.Models
{
    public class GetQuestionsRequest : IPaging
    {
      //  [DisplayFormat(HtmlEncode = true)]
        public string Term { get; set; }
        public int? Page { get; set; }

        [SuppressMessage("ReSharper", "UnusedMember.Global",Justification = "Automapper is injecting this field")]
        public string[] Source { get; set; }

        public QuestionFilter? Filter { get; set; }
    }
}