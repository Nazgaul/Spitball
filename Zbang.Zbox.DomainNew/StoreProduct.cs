

using System;
using System.Collections.Generic;

namespace Zbang.Zbox.Domain
{
    public class StoreProduct
    {
        protected StoreProduct() { }

        public StoreProduct(long id, string name, string extraDetails, int numberOfSales, float coupon, float salePrice,
            string pictureUrl, IList<StoreCategory> categories)
        {
            UpdateProduct(id, name, extraDetails, numberOfSales, coupon, salePrice, pictureUrl, categories);
        }

        public void UpdateProduct(long id, string name, string extraDetails, int numberOfSales, float coupon,
            float salePrice, string pictureUrl, IList<StoreCategory> categories)
        {
            if (name == null) throw new ArgumentNullException("name");
            Id = id;
            Name = name.Trim();
            ExtraDetails = string.IsNullOrEmpty(extraDetails) ? null : extraDetails.Trim();
            NumberOfSales = numberOfSales;
            Coupon = coupon;
            SalePrice = salePrice;
            PictureUrl = pictureUrl;

            FinalPrice = SalePrice - Coupon;
            Discount = (int)(FinalPrice / SalePrice * 100);
            Categories = categories;
        }
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string ExtraDetails { get; set; }

        public virtual int NumberOfSales { get; set; }

        public virtual float Coupon { get; set; }

        public virtual float SalePrice { get; set; }

        public virtual float FinalPrice { get; set; }

        public virtual int Discount { get; set; }

        public virtual string PictureUrl { get; set; }

        public ICollection<StoreCategory> Categories { get; set; }




    }
}
