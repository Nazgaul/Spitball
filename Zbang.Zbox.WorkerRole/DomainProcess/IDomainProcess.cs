
namespace Zbang.Zbox.WorkerRole.DomainProcess
{
   internal interface IDomainProcess
    {
       bool Execute(Infrastructure.Transport.DomainProcess data);
    }
}
