using System;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddLinkToBoxCommand : AddItemToBoxCommand
    {
        public AddLinkToBoxCommand(long userId, long boxId, string url, Guid? tabId, string urlTitle, bool isQuestion)
            : base(userId, boxId)
        {
            IsQuestion = isQuestion;
            Url = url;
            TabId = tabId;
            UrlTitle = urlTitle;
        }

        public string Url { get; private set; }
        public Guid? TabId { get; private set; }

        public string UrlTitle { get; private set; }

        public bool IsQuestion { get; private set; }


        public override string ResolverName
        {
            get { return LinkResolver; }
        }
    }
}
