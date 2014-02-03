
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateUserLanguageCommand : ICommand
    {
        public UpdateUserLanguageCommand(long id, string language)
        {
            // TODO: Complete member initialization
            UserId = id;
            Language = language;
        }

        public long UserId { get; private set; }
        public string Language { get; private set; }
    }
}
