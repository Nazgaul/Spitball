using Cloudents.Core.Enum;

namespace Cloudents.Core.Entities
{
    public interface ISoftDelete
    {
        ItemState State { get;  }
        //void DeleteAssociation();

        bool Delete();
    }
}