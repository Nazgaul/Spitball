using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.WorkerRole.DomainProcess
{
   internal interface IDomainProcess
    {
       bool Excecute(Zbang.Zbox.Infrastructure.Transport.DomainProcess data);
    }
}
