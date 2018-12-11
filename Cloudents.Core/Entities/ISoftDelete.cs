using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Entities
{
    public interface ISoftDelete
    {
        ItemComponent State { get; set; }
        //void DeleteAssociation();
    }
}