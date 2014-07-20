using Zbang.Zbox.Domain.Commands.Store;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers.Store
{
    public class AddProductsToStoreCommandHandler : ICommandHandler<AddProductsToStoreCommand>
    {
        private readonly IRepository<StoreProduct> m_ProductRepository;

        public AddProductsToStoreCommandHandler(IRepository<StoreProduct> productRepository)
        {
            m_ProductRepository = productRepository;
        }

        public void Handle(AddProductsToStoreCommand message)
        {
            foreach (var productStore in message.ProductStores)
            {
                var product = m_ProductRepository.Get(productStore.Id); //use get to get existance in db
                if (product == null)
                {
                    product = new StoreProduct(productStore.Id,
                        productStore.Name,
                        productStore.ExtraDetails,
                        productStore.NumberOfSales,
                        productStore.Coupon,
                        productStore.SalePrice,
                        productStore.PictureUrl);

                }
                else
                {
                    product.UpdateProduct(productStore.Id,
                        productStore.Name,
                        productStore.ExtraDetails,
                        productStore.NumberOfSales,
                        productStore.Coupon,
                        productStore.SalePrice,
                        productStore.PictureUrl);
                }
                m_ProductRepository.Save(product);
            }
        }
    }
}
