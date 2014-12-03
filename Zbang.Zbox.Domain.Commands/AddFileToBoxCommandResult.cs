using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddFileToBoxCommandResult : ICommandResult
    {
        public AddFileToBoxCommandResult(File file)
        {
            File = file;
        }

        public File File { get; private set; }

        //public string[] GetCacheKeys()
        //{
        //    return new[] { ConstCacheKeys.Items + File.Box.Id };
        //}
    }
}
