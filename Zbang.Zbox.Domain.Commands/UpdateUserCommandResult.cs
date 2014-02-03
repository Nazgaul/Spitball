using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateUserCommandResult : ICommandResult
    {
        public UpdateUserCommandResult(string error)
        {   
            Error = error;
        }
        public UpdateUserCommandResult()
        {

        }

        public string Error { get; set; }
        public string Field { get; set; }
        public bool HasErrors
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Error);
            }
        }
    }
}
