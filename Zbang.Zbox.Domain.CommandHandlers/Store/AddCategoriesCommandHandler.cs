using System;
using Zbang.Zbox.Domain.Commands.Store;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers.Store
{
    
    public class AddCategoriesCommandHandler : ICommandHandler<AddCategoriesCommand>
    {
        private readonly IRepository<StoreCategory> m_CategoryRepository;
        public AddCategoriesCommandHandler(IRepository<StoreCategory> categoryRepository)
        {
            m_CategoryRepository = categoryRepository;
        }
        public void Handle(AddCategoriesCommand message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            foreach (var categoryStore in message.Categories)
            {
                var category = m_CategoryRepository.Get(categoryStore.Id); //use get to get existance in db
                if (category == null)
                {
                    category = new StoreCategory(categoryStore.Id, categoryStore.ParentId, categoryStore.Order, categoryStore.Name);
                }
                else
                {
                    category.UpdateCategory(categoryStore.Id, categoryStore.ParentId, categoryStore.Order, categoryStore.Name);
                }
                m_CategoryRepository.Save(category);
            }
        }
    }
}
