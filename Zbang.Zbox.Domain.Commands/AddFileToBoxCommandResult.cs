using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddFileToBoxCommandResult : AddItemToBoxCommandResult
    {
        public AddFileToBoxCommandResult(File file)
        {
            File = file;
        }

        public File File { get; private set; }
    }
}
