using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands.Store
{
    public class AddProductsToStoreCommand : ICommand
    {
        public AddProductsToStoreCommand(IEnumerable<ProductStore> productStores)
        {
            ProductStores = productStores;
        }

        public IEnumerable<ProductStore> ProductStores { get; private set; }
    }

    public class ProductStore
    {
        public ProductStore(long id, string name, string extraDetails, int numberOfSales, float coupon, float salePrice, string pictureUrl)
        {
            Id = id;
            Name = name;
            ExtraDetails = extraDetails;
            NumberOfSales = numberOfSales;
            Coupon = coupon;
            SalePrice = salePrice;
            PictureUrl = pictureUrl;
        }

        public long Id { get;private set; }
        public string Name { get; private set; }
        public string ExtraDetails { get; private set; }

        public int NumberOfSales { get; private set; }

        public float Coupon { get; private set; }

        public float SalePrice { get; private set; }

        public string PictureUrl { get; private set; }
    }
}
