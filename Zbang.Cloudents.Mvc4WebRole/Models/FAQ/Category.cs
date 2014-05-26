using System.Collections.Generic;

namespace Zbang.Cloudents.Mvc4WebRole.Models.FAQ
{
    public class Category
    {
        public int Order { get; set; }
        public string Name { get; set; }
        public string Language { get; set; }

        public IList<QnA> QuestionNAnswers { get; set; }
    }
}