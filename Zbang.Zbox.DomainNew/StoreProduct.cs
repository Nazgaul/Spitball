

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Zbang.Zbox.Infrastructure.Consts;

namespace Zbang.Zbox.Domain
{
    public class StoreProduct
    {
        protected StoreProduct() { }

        public StoreProduct(long id, string name, string extraDetails, int numberOfSales, float coupon,
            float salePrice, string pictureUrl, IList<StoreCategory> categories, string description, bool homePage,
            string supplyTime, int numberOfPayments, string catalogNumber, float deliveryPrice, string producerName,
            IEnumerable<KeyValuePair<string, string>> features)
        {
            NumberOfSales = numberOfSales;
            PictureUrl = pictureUrl;
            UpdateProduct(id, name, extraDetails, coupon, salePrice, categories, description, homePage,
                supplyTime, numberOfPayments, catalogNumber, deliveryPrice, producerName,
                features);
        }

        public void UpdateProduct(long id, string name, string extraDetails, float coupon,
            float salePrice,  IList<StoreCategory> categories, string description,
            bool homePage, string supplyTime, int numberOfPayments, string catalogNumber, float deliveryPrice, string producerName,
            IEnumerable<KeyValuePair<string, string>> features)
        {
            if (name == null) throw new ArgumentNullException("name");
            Id = id;
            Name = name.Trim();
            ExtraDetails = string.IsNullOrEmpty(extraDetails) ? null : extraDetails.Trim();

            Coupon = coupon;
            SalePrice = salePrice;
            
            Url = UrlConsts.BuildStoreProductUrl(Id, Name);
            Categories = categories;

            //foreach (var feature in features)
            //{
            //    var x = new Regex("(\\*)+([^.*?$]+)+(\\*)");
            //    var sPrice = x.Match(feature.Value);
            //    var price = float.Parse(sPrice.Value);

            //    Features.Add(new StoreProductFeatures(feature.Key, feature.Value.Replace("*" + sPrice + "*", string.Empty), price, this));
            //}

            Description = description;
            HomePage = homePage;
            SupplyTime = supplyTime;
            NumberOfPayments = numberOfPayments;
            CatalogNumber = catalogNumber;
            DeliveryPrice = deliveryPrice;
            ProducerName = producerName;
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public string ExtraDetails { get; set; }

        public int NumberOfSales { get; set; }

        public float Coupon { get; set; }

        public float SalePrice { get; set; }

        public string PictureUrl { get; set; }

        public string Url { get; set; }

        private ICollection<StoreCategory> Categories { get; set; }

        private ICollection<StoreProductFeatures> Features { get; set; }

        public string Description { get; set; }
        public bool HomePage { get; set; }
        public string SupplyTime { get; set; }

        public int NumberOfPayments { get; set; }
        public string CatalogNumber { get; set; }
        public float DeliveryPrice { get; set; }

        public string ProducerName { get; set; }

    }
}
