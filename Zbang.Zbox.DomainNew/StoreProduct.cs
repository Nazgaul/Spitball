﻿

using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Consts;

namespace Zbang.Zbox.Domain
{
    public class StoreProduct
    {
        protected StoreProduct() { }

        public StoreProduct(long id, string name, string extraDetails, int numberOfSales, float coupon, float salePrice,
            string pictureUrl, IList<StoreCategory> categories, string description, bool homePage, string supplyTime,
            int numberOfPayments, string catalogNumber, float deliveryPrice)
        {
            UpdateProduct(id, name, extraDetails, numberOfSales, coupon, salePrice, pictureUrl, categories, description, homePage,
                supplyTime, numberOfPayments, catalogNumber, deliveryPrice);
        }

        public void UpdateProduct(long id, string name, string extraDetails, int numberOfSales, float coupon, float salePrice,
            string pictureUrl, IList<StoreCategory> categories, string description, bool homePage, string supplyTime,
            int numberOfPayments, string catalogNumber, float deliveryPrice)
        {
            if (name == null) throw new ArgumentNullException("name");
            Id = id;
            Name = name.Trim();
            ExtraDetails = string.IsNullOrEmpty(extraDetails) ? null : extraDetails.Trim();
            NumberOfSales = numberOfSales;
            Coupon = coupon;
            SalePrice = salePrice;
            PictureUrl = pictureUrl;
            Url = UrlConsts.BuildStoreProductUrl(Id, Name);
            Categories = categories;

            Description = description;
            HomePage = homePage;
            SupplyTime = supplyTime;
            NumberOfPayments = numberOfPayments;
            CatalogNumber = catalogNumber;
            DeliveryPrice = deliveryPrice;
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

        public string Description { get; set; }
        public bool HomePage { get; set; }
        public string SupplyTime { get; set; }

        public int NumberOfPayments { get; set; }
        public string CatalogNumber { get; set; }
        public float DeliveryPrice { get; set; }

    }
}
