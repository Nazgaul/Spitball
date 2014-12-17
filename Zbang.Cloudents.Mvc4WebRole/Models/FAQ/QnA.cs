using System;

namespace Zbang.Cloudents.Mvc4WebRole.Models.FAQ
{
    [Serializable]
    public class QnA
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public int Order { get; set; }
    }
}