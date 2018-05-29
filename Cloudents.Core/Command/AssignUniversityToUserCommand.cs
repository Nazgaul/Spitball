using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Automapper")]
    [SuppressMessage("VS","RCS1170",Justification = "Automapper")]
    public class AssignUniversityToUserCommand : ICommand
    {
        public long UserId { get; private set; }
        public long UniversityId { get; private set; }
    }
}