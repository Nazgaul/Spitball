

namespace Zbang.Zbox.Domain
{
    public class StoreProduct
    {
        protected StoreProduct() { }

        public StoreProduct(long id, string name, string extraDetails, int numberOfSales, float coupon, float salePrice, string pictureUrl)
        {
            UpdateProduct(id, name, extraDetails, numberOfSales, coupon, salePrice, pictureUrl);
        }

        public void UpdateProduct(long id, string name, string extraDetails, int numberOfSales, float coupon,
            float salePrice, string pictureUrl)
        {
            Id = id;
            Name = name;
            ExtraDetails = extraDetails;
            NumberOfSales = numberOfSales;
            Coupon = coupon;
            SalePrice = salePrice;
            PictureUrl = pictureUrl;
        }
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string ExtraDetails { get; set; }

        public virtual int NumberOfSales { get; set; }

        public virtual float Coupon { get; set; }

        public virtual float SalePrice { get; set; }

        public virtual string PictureUrl { get; set; }


    }
}
