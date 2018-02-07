namespace Cloudents.Core.Entities.Db
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
        void DeleteAssociation();
    }
}
