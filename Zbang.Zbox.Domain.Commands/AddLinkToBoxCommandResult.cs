using System.Runtime.Serialization;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    [DataContract]
    public class AddLinkToBoxCommandResult : ICommandResult
    {
        public AddLinkToBoxCommandResult(Item link)
        {
            Link = link;
        }

        public Item Link { get; private set; }

        public Comment NewComment { get; private set; }

        //public string[] GetCacheKeys()
        //{
        //    return new[] { ConstCacheKeys.Items + Link.Box.Id };
        //}
    }
}
