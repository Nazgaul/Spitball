using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class ChangeFileNameCommandResult : ICommandResult
    {
        public ChangeFileNameCommandResult(Item file)
        {
            File = file;
        }

        public Item File { get; set; }

        //public string[] GetCacheKeys()
        //{
        //    return new[] { ConstCacheKeys.Items + File.Box.Id };
        //}
    }
}
