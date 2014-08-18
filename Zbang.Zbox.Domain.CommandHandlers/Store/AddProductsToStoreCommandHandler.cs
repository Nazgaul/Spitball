using System.Collections.Generic;
using System.Linq;
using Zbang.Zbox.Domain.Commands.Store;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Domain.CommandHandlers.Store
{
    public class AddProductsToStoreCommandHandler : ICommandHandler<AddProductsToStoreCommand>
    {
        private readonly IRepository<StoreProduct> m_ProductRepository;
        private readonly IRepository<StoreCategory> m_CategoryRepository;

        public AddProductsToStoreCommandHandler(IRepository<StoreProduct> productRepository,
            IRepository<StoreCategory> categoryRepository
            )
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
            foreach (var productStore in message.ProductStores.Distinct())
            {
                TraceLog.WriteInfo("Process product id: " + productStore.Id);
                var product = m_ProductRepository.Get(productStore.Id); //use get to get existence in db

                if (product == null)
                {
                    if (productStore.IsActive)
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
                        productStore.DeliveryPrice,
                        productStore.ProducerName,
                        productStore.Upgrades, productStore.UniversityId, productStore.CategoryOrder, productStore.Order, productStore.ProducerId);

                }
                else
                {
                    if (productStore.IsActive)
                    {
                        product.UpdateProduct(productStore.Id,
                            productStore.Name,
                            productStore.ExtraDetails,
                            productStore.Coupon,
                            productStore.SalePrice,
                            GetProductCategory(productStore.Categories),
                            productStore.Featured,
                            productStore.SupplyTime,
                            productStore.ProductPayment,
                            productStore.CatalogNumber,
                            productStore.DeliveryPrice,
                            productStore.ProducerName,
                            productStore.Upgrades, productStore.UniversityId, productStore.Description, productStore.CategoryOrder,
                            productStore.Order, productStore.PictureUrl, productStore.ProducerId);
                    }
                    else
                    {
                        m_ProductRepository.Delete(product);
                        continue;
                    }
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
