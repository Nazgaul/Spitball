using System.Threading.Tasks;
using Zbang.Zbox.Store.Dto;

namespace Zbang.Zbox.Store.Services
{
    public interface IWriteService
    {
        Task<OrderDto> InsertOrder(OrderSubmitDto order);
    }
}
