
using System.Threading.Tasks;

namespace Zbang.Zbox.WorkerRole.DomainProcess
{
   internal interface IDomainProcess
    {
       Task<bool> ExecuteAsync(Infrastructure.Transport.DomainProcess data);
    }
}
