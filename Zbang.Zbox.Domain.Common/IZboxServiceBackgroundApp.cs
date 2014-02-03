using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;

namespace Zbang.Zbox.Domain.Common
{
    public interface IZboxServiceBackgroundApp
    {
        void CreateUniversity(CreateUniversityCommand command);
    }
}
