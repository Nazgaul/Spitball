using System.Collections.Generic;


namespace Zbang.Zbox.Infrastructure.Query
{
    public interface IComposite<T> 
    {
        long Id { get; }
        long? ParentId { get; }
        List<T> Replies { get; set; }
    }
}
