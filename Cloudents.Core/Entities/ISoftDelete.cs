using Cloudents.Core.Enum;

namespace Cloudents.Core.Entities
{
    public interface ISoftDelete
    {
        ItemState State { get; set; }
        //void DeleteAssociation();
    }
}