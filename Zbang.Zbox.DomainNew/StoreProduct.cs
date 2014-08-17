

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Zbang.Zbox.Infrastructure.Consts;

namespace Zbang.Zbox.Domain
{
    public class StoreProduct
    {
        protected StoreProduct()
        {
            Features = new Iesi.Collections.Generic.HashedSet<StoreProductFeatures>();
        }

        public StoreProduct(long id, string name, string extraDetails, int numberOfSales, float coupon, float salePrice, string pictureUrl, IList<StoreCategory> categories, string description, bool homePage, string supplyTime, int numberOfPayments, string catalogNumber, float deliveryPrice, string producerName, IEnumerable<KeyValuePair<string, string>> features, int? universityId, int categoryOrder, int productOrder, int? producerId)
            : this()
        {
            NumberOfSales = numberOfSales;
           
           

            UpdateProduct(id, name, extraDetails, coupon, salePrice, categories,  homePage,
                supplyTime, numberOfPayments, catalogNumber, deliveryPrice, producerName,
                features, universityId, description, categoryOrder, productOrder, pictureUrl, producerId);
        }

        public void UpdateProduct(long id, string name, string extraDetails, float coupon, float salePrice, IList<StoreCategory> categories, bool homePage, string supplyTime, int numberOfPayments, string catalogNumber, float deliveryPrice, string producerName, IEnumerable<KeyValuePair<string, string>> features, int? universityId, string description, int categoryOrder, int productOrder, string pictureUrl, int? producerId)
        {
            if (name == null) throw new ArgumentNullException("name");
            Id = id;
            Name = name.Trim();
            ExtraDetails = string.IsNullOrEmpty(extraDetails) ? null : extraDetails.Trim();
            PictureUrl = pictureUrl;
            Coupon = coupon;
            SalePrice = salePrice;
            Description = description;
            Url = UrlConsts.BuildStoreProductUrl(Id, Name);
            Categories = categories;

            Features.Clear();
            if (features != null)
                AddFeatures(features);

            HomePage = homePage;
            SupplyTime = supplyTime;
            NumberOfPayments = numberOfPayments;
            CatalogNumber = catalogNumber;
            DeliveryPrice = deliveryPrice;
            ProducerName = producerName;
            UniversityId = universityId;

            CategoryOrder = categoryOrder;
            ProductOrder = productOrder;
            ProducerId = producerId;
        }

        private void AddFeatures(IEnumerable<KeyValuePair<string, string>> features)
        {
            foreach (var feature in features)
            {
                var categoryOptions = feature.Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var categoryOption in categoryOptions)
                {
                    var x = new Regex("\\*([^*]*)\\*");
                    var sPrice = x.Match(categoryOption).Value.Replace("*", string.Empty);

                    float? price = TryParseNullableFloat(sPrice);

                    Features.Add(new StoreProductFeatures(feature.Key, categoryOption.Replace("*" + sPrice + "*", string.Empty),
                        price, this));
                }


            }
        }

        private static float? TryParseNullableFloat(string s)
        {
            float f;
            if (float.TryParse(s, out f)) return f;
            return null;
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

        public IReadOnlyCollection<StoreProductFeatures> FeaturesReadOnly
        {
            get { return Features.ToList().AsReadOnly(); }
        }

        public string Description { get; set; }
        public bool HomePage { get; set; }
        public string SupplyTime { get; set; }

        public int NumberOfPayments { get; set; }
        public string CatalogNumber { get; set; }
        public float DeliveryPrice { get; set; }

        public string ProducerName { get; set; }

        public int? UniversityId { get; set; }

        public int CategoryOrder { get; set; }
        public int ProductOrder { get; set; }

        public int? ProducerId { get; set; }

    }
}
