using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Query
{
    public interface IUniversityWithCode
    {
        Task<IEnumerable<long>> GetUniversityWithCode();
    }
}
