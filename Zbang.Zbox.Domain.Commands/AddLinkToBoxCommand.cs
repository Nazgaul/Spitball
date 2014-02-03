using System;
using System.Runtime.Serialization;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddLinkToBoxCommand : ICommand
    {
        public AddLinkToBoxCommand(long userId, long boxId, string url, Guid? tabId,string urlTitle)
        {
            UserId = userId;
            BoxId = boxId;
            Url = url;
            TabId = tabId;
            UrlTitle = urlTitle;
        }

        public long UserId { get; private set; }
        public long BoxId { get; private set; }
        public string Url { get; private set; }
        public Guid? TabId { get; private set; }

        public string UrlTitle { get; private set; }
        
    }
}
