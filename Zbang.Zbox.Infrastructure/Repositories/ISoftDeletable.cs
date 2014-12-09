
namespace Zbang.Zbox.Infrastructure.Repositories
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
    }
}
