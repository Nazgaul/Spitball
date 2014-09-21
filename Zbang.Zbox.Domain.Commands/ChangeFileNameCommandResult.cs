using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class ChangeFileNameCommandResult : ICommandResult
    {
        public ChangeFileNameCommandResult(string name, string url)
        {
            Url = url;
            Name = name;
        }

        public string Name { get; private set; }

        public string Url { get; private set; }

        //public string[] GetCacheKeys()
        //{
        //    return new[] { ConstCacheKeys.Items + File.Box.Id };
        //}
    }
}
