
namespace Zbang.Zbox.Infrastructure.UnitsOfWork
{
    public interface IUnitOfWorkImplementer: IUnitOfWork
    {
        void IncrementUsages();
    }
}
