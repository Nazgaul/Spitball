
namespace Zbang.Zbox.Infrastructure.UnitsOfWork
{
    public interface ILocalData
    {
        object this[string key] { get; set; }
        int Count { get; }
        void Clear();
    }
}