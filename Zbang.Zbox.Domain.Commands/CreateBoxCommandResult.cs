
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateBoxCommandResult: ICommandResult
    {
        public CreateBoxCommandResult( long id, string url)
        {
            Url = url;
            Id = id;
        }

        public long Id { get; private set; }
        public string Url  { get; private set; }
        //public Box NewBox { get; private set; }

    }
}
