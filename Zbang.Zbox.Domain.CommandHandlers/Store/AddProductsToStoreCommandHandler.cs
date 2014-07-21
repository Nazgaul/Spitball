using System.Collections.Generic;
using Zbang.Zbox.Domain.Commands.Store;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers.Store
{
    public class AddProductsToStoreCommandHandler : ICommandHandler<AddProductsToStoreCommand>
    {
        private readonly IRepository<StoreProduct> m_ProductRepository;
        private readonly IRepository<StoreCategory> m_CategoryRepository;

        public AddProductsToStoreCommandHandler(IRepository<StoreProduct> productRepository,
            IRepository<StoreCategory> categoryRepository)
        {
            m_ProductRepository = productRepository;
            m_CategoryRepository = categoryRepository;
        }

        public void Handle(AddProductsToStoreCommand message)
        {
            if (message == null)
            {
                throw new System.ArgumentNullException("message");
            }
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
                        productStore.PictureUrl,
                        GetProductCategory(productStore.Categories),
                        productStore.Description,
                        productStore.Featured,
                        productStore.SupplyTime,
                        productStore.ProductPayment,
                        productStore.CatalogNumber,
                        productStore.DeliveryPrice);

                }
                else
                {
                    product.UpdateProduct(productStore.Id,
                        productStore.Name,
                        productStore.ExtraDetails,
                        productStore.NumberOfSales,
                        productStore.Coupon,
                        productStore.SalePrice,
                        productStore.PictureUrl,
                        GetProductCategory(productStore.Categories),
                        productStore.Description,
                        productStore.Featured,
                        productStore.SupplyTime,
                        productStore.ProductPayment,
                        productStore.CatalogNumber,
                        productStore.DeliveryPrice);
                }
                m_ProductRepository.Save(product);
            }
        }

        private IList<StoreCategory> GetProductCategory(string categoryString)
        {
            var categories = categoryString.Split(new[] { '-' }, System.StringSplitOptions.RemoveEmptyEntries);
            var retVal = new List<StoreCategory>();
            foreach (string category in categories)
            {
                int id;
                if (!int.TryParse(category, out id)) continue;
                var categoryElement = m_CategoryRepository.Get(id);
                if (categoryElement == null)
                {
                    continue;
                }
                retVal.Add(categoryElement);
            }

            return retVal;

        }
    }
}
