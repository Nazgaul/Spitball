namespace Zbang.Zbox.Infrastructure.Repositories
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
        void DeleteAssociation();
    }
}
