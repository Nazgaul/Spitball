
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands.Quiz 
{
    public class SaveQuizCommandResult : ICommandResult
    {
        public SaveQuizCommandResult(string url)
        {
            Url = url;
        }
        public string Url { get; private set; }   
    }
}
