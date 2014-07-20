using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands.Store
{
    public class AddCategoriesCommand : ICommand
    {
        public AddCategoriesCommand(IEnumerable<Category> category)
        {
            Categories = category;
        }

        public IEnumerable<Category> Categories { get; private set; }
    }

    public class Category 
    {
        public Category(int id, int parentId, string name, int order)
        {
            Id = id;
            ParentId = parentId;
            Name = name;
            Order = order;
           
        }

        public int Id { get; private set; }
        public int ParentId { get; private set; }

        public string Name { get; private set; }
        public int Order { get; private set; }

        
    }
}
