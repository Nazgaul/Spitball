
namespace Zbang.Zbox.WorkerRole.DomainProcess
{
   internal interface IDomainProcess
    {
       bool Excecute(Infrastructure.Transport.DomainProcess data);
    }
}
