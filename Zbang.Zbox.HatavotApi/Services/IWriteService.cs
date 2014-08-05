using Zbang.Zbox.Store.Dto;

namespace Zbang.Zbox.Store.Services
{
    public interface IWriteService
    {
        void InsertOrder(OrderSubmitDto order);
    }
}
