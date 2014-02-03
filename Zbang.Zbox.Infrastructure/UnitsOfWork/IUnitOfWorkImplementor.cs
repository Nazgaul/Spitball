
namespace Zbang.Zbox.Infrastructure.UnitsOfWork
{
    public interface IUnitOfWorkImplementor: IUnitOfWork
    {
        void IncrementUsages();
    }
}
