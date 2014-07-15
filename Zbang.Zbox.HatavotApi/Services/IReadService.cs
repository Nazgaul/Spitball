using System.Collections.Generic;
using System.Threading.Tasks;
using Zbang.Zbox.Store.Dto;

namespace Zbang.Zbox.Store.Services
{
    public interface IReadService
    {
        Task<IEnumerable<StoreDto>> ReadData();
    }
}
