using Zbang.Zbox.Infrastructure.Consts;

namespace Zbang.Zbox.Domain
{
   public  class StoreCategory
    {
       protected StoreCategory()
       {
           
       }

       public StoreCategory(int id, int parentId, int order, string name)
       {
           UpdateCategory(id, parentId, order, name);
       }

       public void UpdateCategory(int id, int parentId, int order, string name)
       {
           Id = id;
           ParentId = parentId;
           Order = order;
           Name = name;
       }


       public virtual int Id { get; set; }
       public virtual int ParentId { get; set; }
       public virtual string Name { get; set; }
       public virtual int Order { get; set; }

    }
}
