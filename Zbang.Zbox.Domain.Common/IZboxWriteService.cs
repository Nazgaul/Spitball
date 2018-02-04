using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;

namespace Zbang.Zbox.Domain.Common
{
    public interface IZboxWriteService
    {

        void RemoveOldConnections(RemoveOldConnectionCommand command);
    }
}
