using Cloudents.Domain.Entities;

namespace Cloudents.Domain.Interfaces
{
    public interface ISoftDelete
    {
        ItemComponent Item { get; set; }
        void DeleteAssociation();
    }
}