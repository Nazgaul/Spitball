using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Commands.Quiz;

namespace Zbang.Zbox.Domain.Common
{
    public interface IZboxWriteService
    {
        Task DeleteItemAsync(DeleteItemCommand command);

        void RemoveOldConnections(RemoveOldConnectionCommand command);
    }
}
