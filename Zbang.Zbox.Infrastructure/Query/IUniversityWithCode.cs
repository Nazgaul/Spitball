using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Query
{
    public interface IUniversityWithCode
    {
        Task<IEnumerable<long>> GetUniversityWithCodeAsync();
    }
}
