using Cloudents.Core.Entities.Db;

namespace Cloudents.Core.Entities
{
    public interface ISoftDelete
    {
        ItemComponent Item { get; set; }
        //void DeleteAssociation();
    }
}